using Entities.Models;
using Membership.Repositories.QuocDT.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class PageIssueRepository : GenericRepository<PageIssue>
    {
        public PageIssueRepository() { }
        public PageIssueRepository(MangaPublishDBContext context) : base(context) => _context = context;

        public async Task<List<PageIssue>> GetIssuesAsync(int? chapterId = null, bool includeDeleted = false)
        {
            var query = _context.PageIssues.AsQueryable();

            if (!includeDeleted)
            {
                query = query.Where(i => i.Isdeleted == false || i.Isdeleted == null);
            }

            if (chapterId.HasValue)
            {
                var pageIds = await _context.Pages
                    .Where(p => p.Chapterid == chapterId.Value)
                    .Select(p => p.Pageid)
                    .ToListAsync();

                query = query.Where(i => pageIds.Contains(i.Pageid));
            }

            return await query.ToListAsync();
        }

        public async Task<PageIssue> GetIssueByIdAsync(int id, bool includeDeleted = false)
        {
            var query = _context.PageIssues.AsQueryable();
            if (!includeDeleted)
            {
                query = query.Where(i => i.Isdeleted == false || i.Isdeleted == null);
            }

            return await query.FirstOrDefaultAsync(i => i.Issueid == id);
        }

        public async Task<List<PageIssue>> GetByPageIdAsync(int pageId, bool includeDeleted = false)
        {
            var query = _context.PageIssues.Where(i => i.Pageid == pageId);
            if (!includeDeleted)
            {
                query = query.Where(i => i.Isdeleted == false || i.Isdeleted == null);
            }
            return await query.ToListAsync();
        }
    }
}
