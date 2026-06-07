using Entities.Models;

namespace Services.Interface
{
    public interface IAuthService
    {
        Task<User> GetUserAccount(string userName, string password);
        Task<User> GetUserByUsername(string userName);
        Task<int> CreateUser(User user);
        Task<int> CreateUserToken(UserToken token);
        Task<UserToken> GetUserToken(string token);
        Task<int> RevokeUserToken(UserToken token);
    }
}