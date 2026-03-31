namespace HubAdminPanel.Core.Common
{
    /// <summary>
    /// A generic container used to wrap paginated data collections across the Core layer.
    /// Provides essential metadata for managing large datasets through pagination.
    /// </summary>
    /// <typeparam name="T">The type of the entity or object being paginated.</typeparam>
    /// <remarks>
    /// This class is typically used by repositories or handlers to return 
    /// a subset of data along with its total count for UI rendering.
    /// </remarks>
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }


        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        public int ActiveCount {  get; set; }
        public int AdminCount { get; set; }
        public Dictionary<string, int> RoleCounts { get; set; }
    }
}
