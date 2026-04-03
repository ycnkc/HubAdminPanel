using HubAdminPanel.Core.Features.Endpoints.Commands;
using HubAdminPanel.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class DeleteEndpointCommandHandler : IRequestHandler<DeleteEndpointCommand, bool>
{
    private readonly IAppDbContext _context;
    public DeleteEndpointCommandHandler(IAppDbContext context) => _context = context;

    public async Task<bool> Handle(DeleteEndpointCommand request, CancellationToken cancellationToken)
    {
        var endpoint = await _context.Endpoints
            .Include(e => e.EndpointRoleMappings)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (endpoint == null) return false;

        if (endpoint.EndpointRoleMappings.Any())
        {
            _context.EndpointRoleMappings.RemoveRange(endpoint.EndpointRoleMappings);
        }

        _context.Endpoints.Remove(endpoint);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}