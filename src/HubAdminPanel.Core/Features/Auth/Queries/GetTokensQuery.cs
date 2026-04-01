using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubAdminPanel.Core.Features.Auth.Queries
{
    public class GetTokensQuery : IRequest<List<TokenDto>>
    {
        public int UserId { get; set; }
    }

    public class TokenDto 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TokenLastFour { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public bool IsActive { get; set; }
    }
}
