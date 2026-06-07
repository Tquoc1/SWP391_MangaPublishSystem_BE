using Entities.Models;
using Membership.Repositories.QuocDT.Base;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Repository
{
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository() { }
        public UserRepository(MangaPublishDBContext context) : base(context) => _context = context;

        public async Task<User> GetUserById(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Userid == userId);
        }

        public async Task<int> UpdateUser(User user)
        {
            _context.Users.Update(user);
            return await _context.SaveChangesAsync();
        }

        public async Task<MangakaProfile> GetMangakaProfile(int userId)
        {
            return await _context.MangakaProfiles.FirstOrDefaultAsync(x => x.Userid == userId);
        }

        public async Task<AssistantProfile> GetAssistantProfile(int userId)
        {
            return await _context.AssistantProfiles.FirstOrDefaultAsync(x => x.Userid == userId);
        }

        public async Task<int> UpsertMangakaProfile(MangakaProfile profile)
        {
            if (profile.MangakaProfileId == 0)
            {
                _context.MangakaProfiles.Add(profile);
            }
            else
            {
                _context.MangakaProfiles.Update(profile);
            }

            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpsertAssistantProfile(AssistantProfile profile)
        {
            if (profile.AssistantProfileId == 0)
            {
                _context.AssistantProfiles.Add(profile);
            }
            else
            {
                _context.AssistantProfiles.Update(profile);
            }

            return await _context.SaveChangesAsync();
        }
    }
}