using Entities.Models;
using Membership.Repositories.QuocDT.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class PageRepository : GenericRepository<Page>   
    {
        public PageRepository() { }
        public PageRepository(MangaPublishDBContext context) : base(context) => _context = context;

        public async Task<List<Page>> GetPagesAsync(int? chapterId = null, bool? isSentToMangaka = null, int? mangakaId = null, bool includeDeleted = false)
        {
            var query = _context.Pages.AsQueryable();

            if (!includeDeleted)
            {
                query = query.Where(p => p.Isdeleted == false);
            }

            if (chapterId.HasValue)
            {
                query = query.Where(p => p.Chapterid == chapterId.Value);
            }

            if (isSentToMangaka.HasValue)
            {
                query = query.Where(p => p.IsSentToMangaka == isSentToMangaka.Value);
            }

            if (mangakaId.HasValue)
            {
                query = query.Where(p => p.Chapter.Series.Mangakaid == mangakaId.Value);
            }

            return await query.OrderBy(p => p.Pagenumber).ToListAsync();
        }

        public async Task<Page> GetPageByIdAsync(int id, bool includeDeleted = false)
        {
            var query = _context.Pages.AsQueryable();
            if (!includeDeleted)
            {
                query = query.Where(p => p.Isdeleted == false);
            }
            return await query.FirstOrDefaultAsync(p => p.Pageid == id);
        }
    }
}
