using Repositories.Repository;
using DTOs;
using Services.Interface;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var pages = await _pageRepository.GetPagesAsync(chapterId);
            return pages.Select(MapToDto).ToList();
        }

        public async Task<PageDto> GetByIdAsync(int id)
        {
            var page = await _pageRepository.GetPageByIdAsync(id);
            return page != null ? MapToDto(page) : null;
        }

        public async Task<int> CreateAsync(PageDto.Create pageDto, string pageImageUrl)
        {
            var page = new Page
            {
                Chapterid = pageDto.Chapterid,
                Pagenumber = pageDto.Pagenumber,
                Pageimageurl = pageImageUrl,
                Status = "Draft",
                Isdeleted = false
            };

            await _pageRepository.CreateAsync(page);
            return page.Pageid;
        }

        //public async Task<int> UpdateAsync(int id, PageDto.Update pageDto, string pageImageUrl)
        //{
        //    var existing = await _pageRepository.GetByIdAsync(id);
        //    if (existing == null) return 0;

        //    existing.Pagenumber = pageDto.Pagenumber;
        //    existing.Status = pageDto.Status;
        //    existing.Pageimageurl = pageImageUrl;

        //    if (pageDto.Isdeleted.HasValue)
        //    {
        //        existing.Isdeleted = pageDto.Isdeleted;
        //    }

        //    await _pageRepository.UpdateAsync(existing);
        //    return 1;
        //}
        public async Task<int> UpdateAsync(int id, PageDto.Update pageDto, string pageImageUrl)
        {
            var existing = await _pageRepository.GetByIdAsync(id);
            if (existing == null || existing.Isdeleted == true) return 0;

            existing.Pagenumber = pageDto.Pagenumber;

            await _pageRepository.UpdateAsync(existing);
            return 1;
        }
        public async Task<bool> UpdateStatusAsync(int id, PageDto.UpdateStatus dto)
        {
            var existing = await _pageRepository.GetByIdAsync(id);

            if (existing == null || existing.Isdeleted == true)
                return false;

            existing.Status = dto.Status;

            await _pageRepository.UpdateAsync(existing);

            return true;
        }

        public async Task<bool> UploadImageAsync(int id, string pageImageUrl)
        {
            var existing = await _pageRepository.GetByIdAsync(id);

            if (existing == null || existing.Isdeleted == true)
                return false;

            existing.Pageimageurl = pageImageUrl;

            await _pageRepository.UpdateAsync(existing);

            return true;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var existing = await _pageRepository.GetByIdAsync(id);

            if (existing == null || existing.Isdeleted == true)
                return false;

            existing.Isdeleted = true;

            await _pageRepository.UpdateAsync(existing);

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
