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
    public class PageLayerService : IPageLayerService
    {
        private readonly PageLayerRepository _pageLayerRepository; 

        public PageLayerService(PageLayerRepository pageLayerRepository)
        {
            _pageLayerRepository = pageLayerRepository;
        }

        public async Task<List<PageLayerDto>> GetAllAsync(int? pageId)
        {
            return await _pageLayerRepository.GetAllAsync(pageId);
        }

        public async Task<PageLayerDto> GetByIdAsync(int id)
        {
            return await _pageLayerRepository.GetByIdAsync(id);
        }

        public async Task<int> CreateAsync(PageLayerDto.Create dto, string fileStorageUrl)
        {
            return await _pageLayerRepository.CreateWithDefaultsAsync(dto, fileStorageUrl);
        }

        public async Task<int> UpdateAsync(int id, PageLayerDto.Update pageLayerDto, string fileUrl)
        {
            return await _pageLayerRepository.UpdateLayerAsync(id, pageLayerDto, fileUrl);
        }

        public async Task<bool> RemoveAsync(int id)
        {
            return await _pageLayerRepository.DeleteAsync(id);
        }
    }
}
