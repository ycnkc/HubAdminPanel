using Microsoft.EntityFrameworkCore;
using ToDoApi.Data;
using ToDoApi.Models;
using ToDoApi.Repositories;

public class TodoRepository : ITodoRepository //inherits the interface
{
    private readonly AppDbContext _context;

    public TodoRepository(AppDbContext context)
    {
        _context = context; //injects db connection to this class
    }

    public async Task<List<TodoItem>> GetAllAsync()
        => await _context.TodoItems.ToListAsync(); //gets all rows from the db and converts it into C#

    public async Task<TodoItem?> GetByIdAsync(int id)
        => await _context.TodoItems.FindAsync(id);//searches by id

    public async Task AddAsync(TodoItem item)
    {
        _context.TodoItems.Add(item);
        await _context.SaveChangesAsync(); //adds remove add and update commands to the db. 
    }

    public async Task UpdateAsync(TodoItem item)
    {
        _context.TodoItems.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TodoItem item)
    {
        _context.TodoItems.Remove(item);
        await _context.SaveChangesAsync();
    }
}