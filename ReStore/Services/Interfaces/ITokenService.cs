using ReStore.Entities;

namespace ReStore.Services.Interfaces
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(User user);
    }
}
