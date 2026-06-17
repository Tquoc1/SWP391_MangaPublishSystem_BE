using Entities.Models;
using Membership.Repositories.QuocDT.Base;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Repository
{
    public class MangakaAssistantRepository : GenericRepository<MangakaAssistant>
    {
        public MangakaAssistantRepository() { }

        public MangakaAssistantRepository(MangaPublishDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<MangakaAssistant>> GetAllAsync()
        {
            return await _context.MangakaAssistants.ToListAsync();
        }

        public async Task<MangakaAssistant?> GetByIdAsync(int id)
        {
            return await _context.MangakaAssistants
                .FirstOrDefaultAsync(x => x.ContractId == id);
        }

        public async Task<bool> ExistsActiveContractAsync(int mangakaId, int assistantId)
        {
            return await _context.MangakaAssistants.AnyAsync(x =>
                x.MangakaId == mangakaId &&
                x.AssistantId == assistantId &&
                x.Isdeleted != true);
        }

        public async Task<int> AddAsync(MangakaAssistant entity)
        {
            await _context.MangakaAssistants.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.ContractId;
        }

        public async Task<int> UpdateAsync(MangakaAssistant entity)
        {
            _context.MangakaAssistants.Update(entity);
            return await _context.SaveChangesAsync();
        }
    }
}