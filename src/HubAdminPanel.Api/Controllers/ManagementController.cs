using HubAdminPanel.Core.Entities;
using HubAdminPanel.Core.Features.Management.Queries;
using HubAdminPanel.Data;
using MediatR;
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

        public ManagementController(AppDbContext context, IMemoryCache cache, IMediator mediator)
        {
            _context = context;
            _cache = cache;
            _mediator = mediator;
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


    }
}