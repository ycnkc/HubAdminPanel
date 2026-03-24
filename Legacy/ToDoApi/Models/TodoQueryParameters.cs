namespace ToDoApi.Models
{
    public class TodoQueryParameters
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public bool? IsCompleted { get; set; }
        public string SortBy { get; set; } = "Id";
        public bool IsDescending { get; set; } = false;
    }
}
