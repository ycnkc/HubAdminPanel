namespace HubAdminPanel.Core.Entities
{

    public class Token
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TokenHash { get; set; }
        public string TokenLastFour { get; set; }
        public int UserId { get; set; }
        public DateTime ExpireDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }

}
