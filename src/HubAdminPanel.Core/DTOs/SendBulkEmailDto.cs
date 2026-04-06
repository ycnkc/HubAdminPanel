namespace HubAdminPanel.Core.DTOs
{
    public class SendBulkEmailDto
    {
        public List<int> UserIds { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}
