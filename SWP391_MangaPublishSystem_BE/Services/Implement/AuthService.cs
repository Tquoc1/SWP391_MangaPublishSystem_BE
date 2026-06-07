using Entities.Models;
using Repositories.Repository;
using Services.Interface;

namespace Services.Implement
{
    public class AuthService : IAuthService
    {
        private readonly AuthRepository _authRepository;
        public AuthService(AuthRepository authRepository) => _authRepository = authRepository;

        public Task<User> GetUserAccount(string userName, string password)
        {
            return _authRepository.GetUserAccount(userName, password);
        }

        public Task<User> GetUserByUsername(string userName)
        {
            return _authRepository.GetUserByUsername(userName);
        }

        public Task<int> CreateUser(User user)
        {
            return _authRepository.CreateAsync(user);
        }

        public Task<int> CreateUserToken(UserToken token)
        {
            return _authRepository.CreateUserToken(token);
        }

        public Task<UserToken> GetUserToken(string token)
        {
            return _authRepository.GetUserToken(token);
        }

        public Task<int> RevokeUserToken(UserToken token)
        {
            return _authRepository.RevokeUserToken(token);
        }
    }
}