using HubAdminPanel.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HubAdminPanel.Core.Features.Auth.Queries
{
    public class GetTokensQueryHandler : IRequestHandler<GetTokensQuery, List<TokenDto>>
    {
        private readonly IAppDbContext _context;

        public GetTokensQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TokenDto>> Handle(GetTokensQuery request, CancellationToken cancellationToken)
        {
            return await _context.Tokens
                .Where(t => t.UserId == request.UserId)
                .OrderByDescending(t => t.CreatedDate)
                .Select(t => new TokenDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    TokenLastFour = t.TokenLastFour,
                    CreatedDate = t.CreatedDate,
                    ExpireDate = t.ExpireDate,
                    IsActive = t.IsActive
                })
                .ToListAsync(cancellationToken);
        }
    }
}
