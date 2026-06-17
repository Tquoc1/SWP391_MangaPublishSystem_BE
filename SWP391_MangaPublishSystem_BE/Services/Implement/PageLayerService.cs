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
    public class PageLayerService : IPageLayerService
    {
        private readonly PageLayerRepository _pageLayerRepository; 

        public PageLayerService(PageLayerRepository pageLayerRepository)
        {
            _pageLayerRepository = pageLayerRepository;
        }

        public async Task<List<PageLayerDto>> GetAllAsync(int? pageId)
        {
            var layers = await _pageLayerRepository.GetLayersAsync(pageId);
            return layers.Select(MapToDto).ToList();
        }

        public async Task<PageLayerDto> GetByIdAsync(int id)
        {
            var layer = await _pageLayerRepository.GetLayerByIdAsync(id);
            return layer != null ? MapToDto(layer) : null;
        }

        public async Task<int> CreateAsync(PageLayerDto.Create dto, string fileStorageUrl)
        {
            var layer = new Pagelayer
            {
                Pageid = dto.Pageid,
                Uploaderid = dto.Uploaderid,
                Layername = dto.Layername,
                Fileurl = fileStorageUrl,
                Zindex = dto.Zindex ?? 0,
                Versionnumber = 1,
                Isvisible = true,
                Createdat = DateTime.UtcNow
            };

            await _pageLayerRepository.CreateAsync(layer);
            return layer.Layerid;
        }

        public async Task<int> UpdateAsync(int id, PageLayerDto.Update dto, string fileUrl)
        {
            var existing = await _pageLayerRepository.GetByIdAsync(id);
            if (existing == null) return 0;

            existing.Layername = dto.Layername;
            existing.Zindex = dto.Zindex;
            existing.Versionnumber = dto.Versionnumber;
            existing.Isvisible = dto.Isvisible;
            existing.Fileurl = fileUrl;

            await _pageLayerRepository.UpdateAsync(existing);
            return 1;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var existing = await _pageLayerRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _pageLayerRepository.RemoveAsync(existing);
            return true;
        }

        private PageLayerDto MapToDto(Pagelayer layer)
        {
            return new PageLayerDto
            {
                Layerid = layer.Layerid,
                Pageid = layer.Pageid,
                Uploaderid = layer.Uploaderid,
                Layername = layer.Layername,
                Fileurl = layer.Fileurl,
                Zindex = layer.Zindex,
                Versionnumber = layer.Versionnumber,
                Isvisible = layer.Isvisible,
                Createdat = layer.Createdat
            };
        }
    }
}
