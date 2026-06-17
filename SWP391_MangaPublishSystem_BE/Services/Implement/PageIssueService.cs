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
    public class PageIssueService : IPageIssueService
    {
        private readonly PageIssueRepository _pageIssueRepository;

        public PageIssueService(PageIssueRepository pageIssueRepository)
        {
            _pageIssueRepository = pageIssueRepository;
        }

        public async Task<List<PageIssueDto>> GetAllAsync(int? chapterId)
        {
            return await _pageIssueRepository.GetAllActiveAsync(chapterId);
        }

        public async Task<PageIssueDto> GetByIdAsync(int id)
        {
            return await _pageIssueRepository.GetByIdActiveAsync(id);
        }

        public async Task<int> CreateAsync(PageIssueDto.Create pageDto)
        {
            return await _pageIssueRepository.CreateWithDefaultsAsync(pageDto);
        }

        public async Task<int> UpdateAsync(int id, PageIssueDto.Update pageDto)
        {
            return await _pageIssueRepository.UpdateIssueAsync(id, pageDto);
        }

        public async Task<bool> RemoveAsync(int id)
        {
            return await _pageIssueRepository.DeleteAsync(id);
        }
    }
}
