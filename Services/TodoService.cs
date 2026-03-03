using ToDoApi.Models;
using ToDoApi.Repositories;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _repository;

    public TodoService(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<TodoItem>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<TodoItem> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<TodoItem> CreateAsync(string title, int userId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new Exception("Title can't be null.");

        var item = new TodoItem
        {
            Title = title,
            IsCompleted = false,
            UserId = userId
        };

        await _repository.AddAsync(item);
        return item;
    }

    public async Task<bool> UpdateAsync(int id, string title, bool isCompleted)
    {
        var item = await _repository.GetByIdAsync(id);

        if (item == null)
            return false;

        item.Title = title;
        item.IsCompleted = isCompleted;

        await _repository.UpdateAsync(item);
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);

        if (item == null)
            return false;

        await _repository.DeleteAsync(item);
        return true;
    }
}