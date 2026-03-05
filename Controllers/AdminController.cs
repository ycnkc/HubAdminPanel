using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoApi.Data;

namespace ToDoApi.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new { u.Id, u.Username, u.Role, u.CreatedDate })
                .ToListAsync();
            return Ok(users);
        }

        
        [HttpDelete("manage/user/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound("Kullanıcı bulunamadı.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok($"{user.Username} isimli kullanıcı ve tüm verileri silindi.");
        }
    }
}
