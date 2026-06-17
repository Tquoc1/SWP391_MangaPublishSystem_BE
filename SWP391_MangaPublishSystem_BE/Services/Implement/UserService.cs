using Entities.Models;
using Repositories.Repository;
using Services.Interface;
using System.Threading.Tasks;

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

        public async Task<int> UpdateUser(User user)
        {
            await _userRepository.UpdateAsync(user);
            return 1;
        }

        public Task<MangakaProfile> GetMangakaProfile(int userId)
        {
            return _userRepository.GetMangakaProfile(userId);
        }

        public Task<AssistantProfile> GetAssistantProfile(int userId)
        {
            return _userRepository.GetAssistantProfile(userId);
        }

        public async Task<int> UpsertMangakaProfile(MangakaProfile profile)
        {
            if (profile.MangakaProfileId == 0)
            {
                await _userRepository.AddMangakaProfile(profile);
            }
            else
            {
                await _userRepository.UpdateMangakaProfile(profile);
            }
            return 1;
        }

        public async Task<int> UpsertAssistantProfile(AssistantProfile profile)
        {
            if (profile.AssistantProfileId == 0)
            {
                await _userRepository.AddAssistantProfile(profile);
            }
            else
            {
                await _userRepository.UpdateAssistantProfile(profile);
            }
            return 1;
        }
    }
}