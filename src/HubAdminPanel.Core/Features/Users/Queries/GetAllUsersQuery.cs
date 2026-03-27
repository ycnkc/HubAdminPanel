using HubAdminPanel.Core.Common;
using HubAdminPanel.Core.DTOs;
using MediatR;
using System.Collections.Generic;

namespace HubAdminPanel.Core.Features.Users.Queries
{
    /// <summary>
    /// Represents a query to retrieve a paginated list of all users.
    /// Returns a <see cref="PagedResult{UserDto}"/> containing the user profiles and pagination metadata.
    /// </summary>
    /// <remarks>
    /// This query follows the CQRS pattern to separate read operations from write operations,
    /// ensuring efficient data retrieval.
    /// </remarks>
    public class GetAllUsersQuery : IRequest<PagedResult<UserDto>> 
    {
        public int PageNumber { get; set; } = 1; 
        public int PageSize { get; set; } = 5;   
    }
}