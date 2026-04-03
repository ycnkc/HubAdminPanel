using HubAdminPanel.Core.Entities;
using HubAdminPanel.Data;
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

        public ManagementController(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet("endpoints")]
        public async Task<IActionResult> GetEndpoints()
        {
            try
            {
                var endpoints = await _context.Endpoints
                    .Include(e => e.EndpointRoleMappings)
                        .ThenInclude(m => m.Role)
                    .AsNoTracking()
                    .Select(e => new {
                        e.Id,
                        e.Path,
                        e.Method,
                        e.Description,
                        EndpointRoleMappings = e.EndpointRoleMappings.Select(m => new {
                            m.RoleId,
                            Role = new
                            {
                                Name = m.Role.Name
                            }
                        }).ToList()
                    })
                    .ToListAsync();

                return Ok(endpoints);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
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

            // Cache'i sil ki kullanıcının yetkisi anında düşsün!
            _cache.Remove("AllEndpoints");

            return Ok(new { message = "Rol eşleşmesi başarıyla silindi." });
        }
    }
}