using HubAdminPanel.Core.Entities;
using HubAdminPanel.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HubAdminPanel.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ManagementController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("endpoints")]
        public async Task<IActionResult> GetEndpoints()
        {
            var endpoints = await _context.Endpoints.ToListAsync();
            return Ok(endpoints);
        }

        [HttpPost("assign-permission")]
        public async Task<IActionResult> AssignPermission(int endpointId, int permissionId)
        {
            var exists = await _context.EndpointPermissionMappings
                .AnyAsync(m => m.EndpointId == endpointId && m.PermissionId == permissionId);

            if (exists) return BadRequest("Bu yetki zaten bu endpoint'e atanmış.");

            var mapping = new EndpointPermissionMapping
            {
                EndpointId = endpointId,
                PermissionId = permissionId
            };

            _context.EndpointPermissionMappings.Add(mapping);
            await _context.SaveChangesAsync();

            return Ok("Yetki başarıyla eşleştirildi.");
        }

        [HttpDelete("remove-permission")]
        public async Task<IActionResult> RemovePermission(int endpointId, int permissionId)
        {
            var mapping = await _context.EndpointPermissionMappings
                .FirstOrDefaultAsync(m => m.EndpointId == endpointId && m.PermissionId == permissionId);

            if (mapping == null) return NotFound("Eşleşme bulunamadı.");

            _context.EndpointPermissionMappings.Remove(mapping);
            await _context.SaveChangesAsync();

            return Ok("Yetki bu endpoint'ten kaldırıldı.");
        }
    }
}
