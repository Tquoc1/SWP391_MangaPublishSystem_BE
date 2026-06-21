using Entities.Models;
using Services.DTO;

namespace Services.Interface
{
    public interface IUserService
    {
        Task<User> GetUserById(int userId);
        Task<int> UpdateUser(User user);
        Task<MangakaProfile> GetMangakaProfile(int userId);
        Task<AssistantProfile> GetAssistantProfile(int userId);
        Task<int> AddMangakaProfile(MangakaProfile profile);
        Task<int> AddAssistantProfile(AssistantProfile profile);
        Task<int> UpdateMangakaProfile(int userId, UserDto.UpdateMangakaProfile profile, string? avatarUrl);
        Task<int> UpdateAssistantProfile(int userId, UserDto.UpdateAssistantProfile profile, string? avatarUrl);

    }
}