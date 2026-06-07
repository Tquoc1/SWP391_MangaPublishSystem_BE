using Repositories.Repository;
using Services.DTO;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class PageService : IPageService
    {
        private readonly PageRepository _pageRepository;

        public PageService(PageRepository pageRepository)
        {
            _pageRepository = pageRepository;
        }

        public async Task<List<PageDto>> GetAllAsync(int? chapterId)
        {
            var pages = await _pageRepository.GetAllAsync();

            var query = pages.Where(p => p.Isdeleted == false);

            if (chapterId.HasValue)
            {
                query = query.Where(p => p.Chapterid == chapterId.Value);
            }

            return query
                .OrderBy(p => p.Pagenumber)
                .Select(p => new PageDto
                {
                    Pageid = p.Pageid,
                    Chapterid = p.Chapterid,
                    Pagenumber = p.Pagenumber,
                    Pageimageurl = p.Pageimageurl,
                    Status = p.Status,
                    Isdeleted = p.Isdeleted
                }).ToList();
        }

        public async Task<PageDto> GetByIdAsync(int id)
        {
            var p = await _pageRepository.GetByIdAsync(id);
            if (p == null) return null;

            return new PageDto
            {
                Pageid = p.Pageid,
                Chapterid = p.Chapterid,
                Pagenumber = p.Pagenumber,
                Pageimageurl = p.Pageimageurl,
                Status = p.Status,
                Isdeleted = p.Isdeleted
            };
        }

        public Task<int> CreateAsync(PageDto.Create pageDto, string pageImageUrl)
        {
            var page = new Entities.Models.Page
            {
                Chapterid = pageDto.Chapterid,
                Pagenumber = pageDto.Pagenumber,
                Pageimageurl = pageImageUrl,     
                Status = "Draft",                 
                Isdeleted = false
            };
            return _pageRepository.CreateAsync(page);
        }

        public async Task<int> UpdateAsync(int id, PageDto.Update pageDto, string pageImageUrl)
        {
            var existing = await _pageRepository.GetByIdAsync(id);
            if (existing == null) return 0;

            existing.Pagenumber = pageDto.Pagenumber;
            existing.Status = pageDto.Status;
            existing.Pageimageurl = pageImageUrl; 

            if (pageDto.Isdeleted.HasValue)
            {
                existing.Isdeleted = pageDto.Isdeleted;
            }

            return await _pageRepository.UpdateAsync(existing);
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var existing = await _pageRepository.GetByIdAsync(id);
            if (existing == null) return false;

            return await _pageRepository.RemoveAsync(existing);
        }
    }
}
