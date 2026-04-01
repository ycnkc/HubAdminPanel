using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubAdminPanel.Core.Features.Auth.Commands
{
    public class GenerateTokenCommand : IRequest<object> 
    {
        public string Name { get; set; }
        public int ExpireDays { get; set; }
        public int UserId { get; set; } 
    }
}
