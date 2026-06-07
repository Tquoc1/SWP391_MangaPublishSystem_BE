using Entities.Models;
using Repositories.Repository;
using Services.Interface;

namespace Services.Implement
{
    public class UserService : IUserService
    {
        private readonly UserRepository _userRepository;
        public UserService(UserRepository userRepository) => _userRepository = userRepository;

        public Task<User> GetUserById(int userId)
        {
            return _userRepository.GetUserById(userId);
        }

        public Task<int> UpdateUser(User user)
        {
            return _userRepository.UpdateUser(user);
        }

        public Task<MangakaProfile> GetMangakaProfile(int userId)
        {
            return _userRepository.GetMangakaProfile(userId);
        }

        public Task<AssistantProfile> GetAssistantProfile(int userId)
        {
            return _userRepository.GetAssistantProfile(userId);
        }

        public Task<int> UpsertMangakaProfile(MangakaProfile profile)
        {
            return _userRepository.UpsertMangakaProfile(profile);
        }

        public Task<int> UpsertAssistantProfile(AssistantProfile profile)
        {
            return _userRepository.UpsertAssistantProfile(profile);
        }
    }
}