using Entities.Models;

namespace Services.Interface
{
    public interface IUserService
    {
        Task<User> GetUserById(int userId);
        Task<int> UpdateUser(User user);
        Task<MangakaProfile> GetMangakaProfile(int userId);
        Task<AssistantProfile> GetAssistantProfile(int userId);
        Task<int> UpsertMangakaProfile(MangakaProfile profile);
        Task<int> UpsertAssistantProfile(AssistantProfile profile);
    }
}