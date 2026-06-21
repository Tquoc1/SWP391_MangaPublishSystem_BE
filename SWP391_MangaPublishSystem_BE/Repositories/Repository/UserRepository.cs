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

        public async Task<MangakaProfile> GetMangakaProfile(int userId)
        {
            return await _context.MangakaProfiles.FirstOrDefaultAsync(x => x.Userid == userId);
        }

        public async Task<AssistantProfile> GetAssistantProfile(int userId)
        {
            return await _context.AssistantProfiles.FirstOrDefaultAsync(x => x.Userid == userId);
        }

        public async Task AddMangakaProfile(MangakaProfile profile)
        {
            _context.MangakaProfiles.Add(profile);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateMangakaProfile(MangakaProfile profile)
        {
            _context.MangakaProfiles.Update(profile);
            await _context.SaveChangesAsync();
        }

        public async Task AddAssistantProfile(AssistantProfile profile)
        {
            _context.AssistantProfiles.Add(profile);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAssistantProfile(AssistantProfile profile)
        {
            _context.AssistantProfiles.Update(profile);
            await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(User entity)
        {
            _context.Users.Update(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<User>> GetAdminsAsync()
        {
            return await _context.Users
                .Where(u => u.Roleid == 1 || u.Roleid == 2 || u.Roleid == 3)
                .ToListAsync();
        }

        public async Task<List<AssistantProfile>> GetAvailableAssistantsAsync()
        {
            return await _context.AssistantProfiles
                .Include(a => a.User)
                .Where(a => a.IsAvailable == true)
                .ToListAsync();
        }
    }
}