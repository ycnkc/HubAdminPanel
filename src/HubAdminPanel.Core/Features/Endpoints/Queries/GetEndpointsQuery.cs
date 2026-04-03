using HubAdminPanel.Core.DTOs;
using MediatR;
using System.Collections.Generic;

namespace HubAdminPanel.Core.Features.Management.Queries
{
    public class GetEndpointsQuery : IRequest<List<EndpointDto>> { }

    public class EndpointDto
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }
        public string Description { get; set; }
        public List<EndpointRoleMappingDto> EndpointRoleMappings { get; set; }
    }

   

}