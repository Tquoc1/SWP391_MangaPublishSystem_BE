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

        public async Task<List<Chapter>> GetChaptersAsync(int? seriesId = null, bool includeDeleted = false)
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
