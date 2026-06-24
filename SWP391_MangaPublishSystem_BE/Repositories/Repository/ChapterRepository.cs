using Entities.Models;
using Membership.Repositories.QuocDT.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class ChapterRepository : GenericRepository<Chapter>
    {
        public ChapterRepository() { }
        public ChapterRepository(MangaPublishDBContext context) : base(context) => _context = context;

        public async Task<List<Chapter>> GetChaptersAsync(int? seriesId = null, string? status = null, bool includeDeleted = false)
        {
            var query = _context.Chapters.AsQueryable();

            if (!includeDeleted)
            {
                query = query.Where(c => c.Isdeleted == false);
            }

            if (seriesId.HasValue)
            {
                query = query.Where(c => c.Seriesid == seriesId.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(c => c.Status == status);
            }

            return await query.ToListAsync();
        }

        public async Task<List<Chapter>> GetChaptersByAssistantIdAsync(int assistantId, bool includeDeleted = false)
        {
            var mangakaIds = await _context.MangakaAssistants
                .Where(ma => ma.AssistantId == assistantId && (ma.Isdeleted == false || ma.Isdeleted == null))
                .Select(ma => ma.MangakaId)
                .Distinct()
                .ToListAsync();

            var query = _context.Chapters.AsQueryable();

            if (!includeDeleted)
            {
                query = query.Where(c => c.Isdeleted == false);
            }

            query = query.Where(c => mangakaIds.Contains(c.Series.Mangakaid));

            return await query.ToListAsync();
        }

        public async Task<Chapter> GetChapterByIdAsync(int id, bool includeDeleted = false)
        {
            var query = _context.Chapters.AsQueryable();
            if (!includeDeleted)
            {
                query = query.Where(c => c.Isdeleted == false);
            }
            return await query.FirstOrDefaultAsync(c => c.Chapterid == id);
        }
    }
}
