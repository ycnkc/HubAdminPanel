using ToDoApi.Models;

public interface ITodoService
{
    Task<List<TodoItem>> GetAllAsync();
    Task<TodoItem> GetByIdAsync(int id);
    Task<TodoItem> CreateAsync(string title, int userId);
    Task<bool> UpdateAsync(int id, string title, bool isCompleted);
    Task<bool> DeleteAsync(int id);
}