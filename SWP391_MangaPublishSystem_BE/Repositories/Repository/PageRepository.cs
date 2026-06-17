using Entities.Models;
using Membership.Repositories.QuocDT.Base;
using Microsoft.EntityFrameworkCore;
using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class PageRepository : GenericRepository<Page>   
    {
        public PageRepository() { }
        public PageRepository(MangaPublishDBContext context) : base(context) => _context = context;


        public async Task<List<PageDto>> GetAllActiveAsync(int? chapterId = null)
        {
            var query = _context.Pages.Where(p => p.Isdeleted == false);

            if (chapterId.HasValue)
            {
                query = query.Where(p => p.Chapterid == chapterId.Value);
            }

            var pages = await query.OrderBy(p => p.Pagenumber).ToListAsync();
            return pages.Select(MapToDto).ToList();
        }


        public async Task<PageDto> GetByIdActiveAsync(int id)
        {
            var page = await _context.Pages
                .FirstOrDefaultAsync(p => p.Pageid == id && p.Isdeleted == false);
            return page != null ? MapToDto(page) : null;
        }


        public async Task<List<PageDto>> GetByChapterIdAsync(int chapterId)
        {
            var pages = await _context.Pages
                .Where(p => p.Chapterid == chapterId && p.Isdeleted == false)
                .OrderBy(p => p.Pagenumber)
                .ToListAsync();
            return pages.Select(MapToDto).ToList();
        }


        public async Task<int> CreateWithDefaultsAsync(int chapterId, int pageNumber, string pageImageUrl)
        {
            var page = new Page
            {
                Chapterid = chapterId,
                Pagenumber = pageNumber,
                Pageimageurl = pageImageUrl,
                Status = "Draft",
                Isdeleted = false
            };

            _context.Pages.Add(page);
            await _context.SaveChangesAsync();
            return page.Pageid;
        }


        public async Task<int> UpdatePageAsync(int id, int pageNumber, string status, string pageImageUrl, bool? isDeleted)
        {
            var existing = await _context.Pages.FindAsync(id);
            if (existing == null) return 0;

            existing.Pagenumber = pageNumber;
            existing.Status = status;
            existing.Pageimageurl = pageImageUrl;

            if (isDeleted.HasValue)
            {
                existing.Isdeleted = isDeleted;
            }

            _context.ChangeTracker.Clear();
            var tracker = _context.Attach(existing);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Pages.FindAsync(id);
            if (existing == null) return false;

            _context.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }


        private PageDto MapToDto(Page page)
        {
            return new PageDto
            {
                Pageid = page.Pageid,
                Chapterid = page.Chapterid,
                Pagenumber = page.Pagenumber,
                Pageimageurl = page.Pageimageurl,
                Status = page.Status,
                Isdeleted = page.Isdeleted
            };
        }
    }
}
