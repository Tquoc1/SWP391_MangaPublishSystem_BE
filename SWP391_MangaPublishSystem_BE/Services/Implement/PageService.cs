using Repositories.Repository;
using DTOs;
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
            return await _pageRepository.GetAllActiveAsync(chapterId);
        }

        public async Task<PageDto> GetByIdAsync(int id)
        {
            return await _pageRepository.GetByIdActiveAsync(id);
        }

        public async Task<int> CreateAsync(PageDto.Create pageDto, string pageImageUrl)
        {
            return await _pageRepository.CreateWithDefaultsAsync(pageDto.Chapterid, pageDto.Pagenumber, pageImageUrl);
        }

        public async Task<int> UpdateAsync(int id, PageDto.Update pageDto, string pageImageUrl)
        {
            return await _pageRepository.UpdatePageAsync(id, pageDto.Pagenumber, pageDto.Status, pageImageUrl, pageDto.Isdeleted);
        }

        public async Task<bool> RemoveAsync(int id)
        {
            return await _pageRepository.DeleteAsync(id);
        }
    }
}
