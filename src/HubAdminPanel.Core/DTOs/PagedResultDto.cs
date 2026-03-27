namespace HubAdminPanel.Core.DTOs
{
    /// <summary>
    /// A generic wrapper for paginated data results. 
    /// This DTO provides metadata for client-side navigation and list rendering.
    /// </summary>
    /// <typeparam name="T">The type of the items in the collection (e.g., UserDto, RoleDto).</typeparam>
    internal class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
