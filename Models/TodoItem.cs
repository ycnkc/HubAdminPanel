namespace ToDoApi.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public string Title { get; set; }
        public bool IsCompleted { get; set; }
    }
}
