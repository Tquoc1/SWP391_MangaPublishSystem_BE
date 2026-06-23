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
                Opacity = dto.Opacity ?? 1.0m,
                Versionnumber = 1,
                Isvisible = true,
                Isdeleted = false,
                Createdat = DateTime.UtcNow
            };

            await _pageLayerRepository.CreateAsync(layer);
            return layer.Layerid;
        }

        public async Task UpdateAsync(int id, PageLayerDto.Update dto, string fileUrl)
        {
            var existing = await _pageLayerRepository.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Không tìm thấy lớp vẽ (Layer) cần cập nhật.");

            existing.Layername = dto.Layername;
            existing.Zindex = dto.Zindex;
            existing.Opacity = dto.Opacity ?? existing.Opacity;
            existing.Versionnumber = dto.Versionnumber;
            
            if (!string.IsNullOrEmpty(fileUrl))
                existing.Fileurl = fileUrl;

            await _pageLayerRepository.UpdateAsync(existing);
        }

        public async Task ToggleVisibilityAsync(int id)
        {
            var existing = await _pageLayerRepository.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Không tìm thấy lớp vẽ.");

            existing.Isvisible = !existing.Isvisible;

            await _pageLayerRepository.UpdateAsync(existing);
        }

        public async Task SoftDeleteAsync(int id)
        {
            var existing = await _pageLayerRepository.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Không tìm thấy lớp vẽ.");

            existing.Isdeleted = true;
            await _pageLayerRepository.UpdateAsync(existing);
        }

        public async Task RemoveAsync(int id)
        {
            var existing = await _pageLayerRepository.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Không tìm thấy lớp vẽ.");

            await _pageLayerRepository.RemoveAsync(existing);
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
                Opacity = layer.Opacity,
                Versionnumber = layer.Versionnumber,
                Isvisible = layer.Isvisible,
                Isdeleted = layer.Isdeleted,
                Createdat = layer.Createdat
            };
        }
    }
}
