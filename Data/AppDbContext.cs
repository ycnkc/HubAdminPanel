using Microsoft.EntityFrameworkCore;
using ToDoApi.Models;

namespace ToDoApi.Data
{
    public class AppDbContext : DbContext //inheriting DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { } //gets db options from Program.cs and sends to base class.
        public DbSet<TodoItem> TodoItems { get; set; } //table in the DB.
    }
} 
