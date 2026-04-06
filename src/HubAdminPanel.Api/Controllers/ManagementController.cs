using HubAdminPanel.Core.DTOs;
using HubAdminPanel.Core.Entities;
using HubAdminPanel.Core.Features.Endpoints.Commands;
using HubAdminPanel.Core.Features.Management.Queries;
using HubAdminPanel.Core.Interfaces;
using HubAdminPanel.Data;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace HubAdminPanel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly IMediator _mediator;
        private readonly IEmailService _emailService;

        public ManagementController(AppDbContext context, IMemoryCache cache, IMediator mediator, IEmailService emailService)
        {
            _context = context;
            _cache = cache;
            _mediator = mediator;
            _emailService = emailService;
        }

        [HttpGet("endpoints")]
        public async Task<IActionResult> GetEndpoints()
        {
            var result = await _mediator.Send(new GetEndpointsQuery());
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return Ok(new List<User>());

            var users = await _context.Users
                .Where(u => u.Username.Contains(query))
                .Select(u => new { u.Id, u.Username })
                .Take(10)
                .ToListAsync();

            return Ok(users);
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromQuery] int endpointId, [FromQuery] int roleId)
        {
            var exists = await _context.EndpointRoleMappings
                .AnyAsync(m => m.EndpointId == endpointId && m.RoleId == roleId);

            if (exists)
                return BadRequest("Bu rol zaten bu endpoint'e atanmış.");

            var mapping = new EndpointRoleMapping
            {
                EndpointId = endpointId,
                RoleId = roleId
            };

            await _context.EndpointRoleMappings.AddAsync(mapping);
            await _context.SaveChangesAsync();

            _cache.Remove("AllEndpoints");

            return Ok(new { message = "Rol başarıyla atandı." });
        }

        [HttpDelete("remove-role-from-endpoint/{endpointId}/{roleId}")]
        public async Task<IActionResult> RemoveRoleFromEndpoint(int endpointId, int roleId)
        {
            var mapping = await _context.EndpointRoleMappings
                .FirstOrDefaultAsync(m => m.EndpointId == endpointId && m.RoleId == roleId);

            if (mapping == null)
                return NotFound("Böyle bir eşleşme bulunamadı.");

            _context.EndpointRoleMappings.Remove(mapping);
            await _context.SaveChangesAsync();

            _cache.Remove("AllEndpoints");

            return Ok(new { message = "Rol eşleşmesi başarıyla silindi." });
        }

        [HttpDelete("endpoints/{id}")]
        public async Task<IActionResult> DeleteEndpoint(int id)
        {
            var result = await _mediator.Send(new DeleteEndpointCommand(id));
            if (result)
            {
                _cache.Remove("AllEndpoints");
                return Ok();
            }
            return NotFound();
        }

        [HttpPost("send-bulk-email")]
        public async Task<IActionResult> SendBulkEmail([FromBody] SendBulkEmailDto dto)
        {
            try
            {
                List<User> users;

                if (dto.UserIds == null || dto.UserIds.Count == 0)
                {
                    users = await _context.Users.ToListAsync();
                }
                else
                {
                    users = await _context.Users
                        .Where(u => dto.UserIds.Contains(u.Id))
                        .ToListAsync();
                }

                int successCount = 0;
                foreach (var user in users)
                {
                    try
                    {
                        var personalizedContent = dto.Content.Replace("{name}", user.Username);
                        await _emailService.SendEmailAsync(user.Email, dto.Subject, personalizedContent);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Mail gönderilemedi ({user.Email}): {ex.Message}");
                    }
                }

                return Ok(new { message = $"{successCount} adet e-posta başarıyla gönderildi." });
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem sırasında bir hata oluştu: " + ex.Message);
            }
        }
    }
    }
