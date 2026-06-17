using Entities.Models;
using Repositories.Repository;
using Services.Interface;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class AuthService : IAuthService
    {
        private readonly AuthRepository _authRepository;
        public AuthService(AuthRepository authRepository) => _authRepository = authRepository;

        public async Task<User> GetUserAccount(string userName, string password)
        {
            // Note: Password verification logic should ideally be here, 
            // but the current controller implementation handles BCrypt.
            // For now, we return the user if they exist.
            return await _authRepository.GetUserByUsername(userName);
        }

        public Task<User> GetUserByUsername(string userName)
        {
            return _authRepository.GetUserByUsername(userName);
        }

        public async Task<int> CreateUser(User user)
        {
            await _authRepository.CreateAsync(user);
            return 1;
        }

        public async Task<int> CreateUserToken(UserToken token)
        {
            await _authRepository.CreateUserToken(token);
            return 1;
        }

        public Task<UserToken> GetUserToken(string token)
        {
            return _authRepository.GetUserToken(token);
        }

        public async Task<int> RevokeUserToken(UserToken token)
        {
            token.Isrevoked = true;
            await _authRepository.UpdateUserToken(token);
            return 1;
        }
    }
}