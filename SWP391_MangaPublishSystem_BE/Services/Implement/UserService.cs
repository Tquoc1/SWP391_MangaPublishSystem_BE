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

        public async Task<int> AddMangakaProfile(MangakaProfile profile)
        {
            await _userRepository.AddMangakaProfile(profile);
            return 1;
        }

        public async Task<int> AddAssistantProfile(AssistantProfile profile)
        {
            await _userRepository.AddAssistantProfile(profile);
            return 1;
        }

        public async Task<int> UpdateMangakaProfile(int userId, DTO.UserDto.UpdateMangakaProfile dto, string? avatarUrl)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user == null) return 0;

            if (!string.IsNullOrWhiteSpace(dto.FullName))
            {
                user.Fullname = dto.FullName;
                await _userRepository.UpdateAsync(user);
            }

            var profile = await _userRepository.GetMangakaProfile(userId);
            if (profile == null) return 0;

            profile.PenName = dto.PenName;
            profile.Bio = dto.Bio;
            profile.PhoneNumber = dto.PhoneNumber;
            profile.BankName = dto.BankName;
            profile.BankAccountNumber = dto.BankAccountNumber;
            profile.BankAccountName = dto.BankAccountName;
            if (!string.IsNullOrEmpty(avatarUrl))
            {
                profile.AvatarUrl = avatarUrl;
            }
            profile.Updatedat = DateTime.UtcNow;

            await _userRepository.UpdateMangakaProfile(profile);
            return 1;
        }

        public async Task<int> UpdateAssistantProfile(int userId, DTO.UserDto.UpdateAssistantProfile dto, string? avatarUrl)
        {
            var user = await _userRepository.GetUserById(userId);
            if (user == null) return 0;

            if (!string.IsNullOrWhiteSpace(dto.FullName))
            {
                user.Fullname = dto.FullName;
                await _userRepository.UpdateAsync(user);
            }

            var profile = await _userRepository.GetAssistantProfile(userId);
            if (profile == null) return 0;

            if (!string.IsNullOrEmpty(avatarUrl))
            {
                profile.AvatarUrl = avatarUrl;
            }
            profile.PortfolioUrl = dto.PortfolioUrl;
            profile.PhoneNumber = dto.PhoneNumber;
            profile.IsAvailable = dto.IsAvailable;
            profile.Skills = dto.Skills;
            profile.SoftwareUsed = dto.SoftwareUsed;
            profile.BankName = dto.BankName;
            profile.BankAccountNumber = dto.BankAccountNumber;
            profile.BankAccountName = dto.BankAccountName;
            profile.Updatedat = DateTime.UtcNow;

            await _userRepository.UpdateAssistantProfile(profile);
            return 1;
        }
    }
}