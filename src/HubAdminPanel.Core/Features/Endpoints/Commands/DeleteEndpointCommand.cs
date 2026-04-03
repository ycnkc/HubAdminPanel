using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubAdminPanel.Core.Features.Endpoints.Commands
{
    public record DeleteEndpointCommand(int Id) : IRequest<bool>;
}
