using HubAdminPanel.Core.DTOs;
using HubAdminPanel.Core.Entities;
using HubAdminPanel.Data;
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
            var endpoints = await _context.Endpoints
                .Include(e => e.EndpointPermissionMappings)
                    .ThenInclude(m => m.Permission)
                .Include(e => e.EndpointUsers)
                    .ThenInclude(u => u.User)
                .ToListAsync();

            return Ok(endpoints);
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

            return Ok(new { message = "Yetki başarıyla eşleştirildi." });
        }

        [HttpDelete("remove-permission-from-endpoint/{endpointId}/{permissionId}")]
        public async Task<IActionResult> RemovePermission(int endpointId, int permissionId)
        {
            var mapping = await _context.EndpointPermissionMappings
                .FirstOrDefaultAsync(m => m.EndpointId == endpointId && m.PermissionId == permissionId);

            if (mapping == null) return NotFound("Eşleşme bulunamadı.");

            _context.EndpointPermissionMappings.Remove(mapping);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Yetki bu endpoint'ten kaldırıldı." });
        }

        [HttpPost("assign-user-to-endpoint")]
        public async Task<IActionResult> AssignUser([FromBody] EndpointUserDto dto)
        {
            var exists = await _context.EndpointUsers
                .AnyAsync(x => x.EndpointId == dto.EndpointId && x.UserId == dto.UserId);

            if (exists) return BadRequest("Bu kullanıcı zaten bu endpoint'e doğrudan erişim yetkisine sahip.");

            var mapping = new EndpointUser { EndpointId = dto.EndpointId, UserId = dto.UserId };
            _context.EndpointUsers.Add(mapping);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kullanıcıya doğrudan erişim izni verildi." });
        }

        [HttpDelete("remove-user-from-endpoint/{endpointId}/{userId}")]
        public async Task<IActionResult> RemoveUser(int endpointId, int userId)
        {
            var mapping = await _context.EndpointUsers
                .FirstOrDefaultAsync(x => x.EndpointId == endpointId && x.UserId == userId);

            if (mapping == null) return NotFound("Eşleşme bulunamadı.");

            _context.EndpointUsers.Remove(mapping);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kullanıcı erişimi başarıyla kaldırıldı." });
        }

        public class EndpointUserDto
        {
            public int EndpointId { get; set; }
            public int UserId { get; set; }
        }

        [HttpPost("assign-users-bulk")]
        public async Task<IActionResult> AssignUsersBulk([FromBody] BulkEndpointUserDto dto)
        {
            if (dto.UserIds == null || !dto.UserIds.Any())
                return BadRequest("Kullanıcı listesi boş olamaz.");

            foreach (var userId in dto.UserIds)
            {
                // Zaten varsa atla (Hata fırlatıp tüm işlemi bozmasın)
                var exists = await _context.EndpointUsers
                    .AnyAsync(x => x.EndpointId == dto.EndpointId && x.UserId == userId);

                if (!exists)
                {
                    _context.EndpointUsers.Add(new EndpointUser
                    {
                        EndpointId = dto.EndpointId,
                        UserId = userId
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Seçilen kullanıcılara erişim yetkisi verildi." });
        }

        public class BulkEndpointUserDto
        {
            public int EndpointId { get; set; }
            public List<int> UserIds { get; set; }
        }
    }
}