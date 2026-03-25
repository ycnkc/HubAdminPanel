using HubAdminPanel.Core.Entities;

namespace HubAdminPanel.Core.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
        public string CreateRefreshToken();
    }
}