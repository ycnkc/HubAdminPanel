using MediatR;
using System.Security.Cryptography;
using System.Text;
using HubAdminPanel.Core.Entities;
using HubAdminPanel.Core.Features.Auth.Commands;
using HubAdminPanel.Core.Interfaces;

namespace HubAdminPanel.Core.Features.Auth.Handlers
{
    public class GenerateTokenCommandHandler : IRequestHandler<GenerateTokenCommand, object>
    {
        private readonly IAppDbContext _context;

        public GenerateTokenCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<object> Handle(GenerateTokenCommand request, CancellationToken cancellationToken)
        {
            string originalToken = "HUB-" + Guid.NewGuid().ToString("N").ToUpper();
            string hashedToken = ComputeSha256Hash(originalToken);
            string lastFour = originalToken.Substring(originalToken.Length - 4);

            var tokenEntity = new Token
            {
                Name = request.Name,
                TokenHash = hashedToken,
                TokenLastFour = lastFour,
                UserId = request.UserId,
                ExpireDate = DateTime.Now.AddDays(request.ExpireDays),
                IsActive = true
            };

            _context.Tokens.Add(tokenEntity);
            await _context.SaveChangesAsync(cancellationToken);

            return new
            {
                Success = true,
                Token = originalToken,
                LastFour = lastFour,
                Message = "Token başarıyla üretildi."
            };
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (var sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                var builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}