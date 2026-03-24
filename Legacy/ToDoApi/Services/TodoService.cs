using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using ToDoApi.Models;
using ToDoApi.Repositories;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _repository;
    private readonly IDistributedCache _cache;

    public TodoService(ITodoRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<List<TodoItem>> GetAllAsync()
    {
        string cacheKey = "all_todos";

        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<List<TodoItem>>(cachedData);
        }

        var todos = await _repository.GetAllAsync();

        var options = new DistributedCacheEntryOptions()
        .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(todos), options);

        return todos;
    }

    public async Task<TodoItem> GetByIdAsync(int id)
    {
        string cacheKey = $"todo_{id}";

        var cachedData = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cachedData))
        {
            if (cachedData == "NOT_FOUND")
            {
                return null;
            }
            return JsonSerializer.Deserialize<TodoItem>(cachedData);
        }

        var todo = await _repository.GetByIdAsync(id);

        if (todo == null)
        {
            await _cache.SetStringAsync(cacheKey, "NOT_FOUND", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            });
            return null;
        }

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(todo), new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });

        return todo;
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
        await _cache.RemoveAsync("all_todos");
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

        await _cache.RemoveAsync("all_todos");
        await _cache.RemoveAsync($"todo_{item.Id}");

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);

        if (item == null)
            return false;

        await _repository.DeleteAsync(item);

        await _cache.RemoveAsync("all_todos");
        await _cache.RemoveAsync($"todo_{item.Id}");

        return true;
    }
}