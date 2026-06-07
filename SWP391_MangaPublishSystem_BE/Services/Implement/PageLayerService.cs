using Entities.Models;
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
    public class PageLayerService : IPageLayerService
    {
        private readonly PageLayerRepository _pageLayerRepository; 

        public PageLayerService(PageLayerRepository pageLayerRepository)
        {
            _pageLayerRepository = pageLayerRepository;
        }

        public async Task<List<PageLayerDto>> GetAllAsync(int? pageId)
        {
            var layers = await _pageLayerRepository.GetAllAsync();

            var query = layers.AsQueryable();

            if (pageId.HasValue)
            {
                query = query.Where(l => l.Pageid == pageId.Value);
            }
            return query
                .OrderBy(l => l.Zindex)
                .Select(l => new PageLayerDto
                {
                    Layerid = l.Layerid,
                    Pageid = l.Pageid,
                    Uploaderid = l.Uploaderid,
                    Layername = l.Layername,
                    Fileurl = l.Fileurl,
                    Zindex = l.Zindex,
                    Versionnumber = l.Versionnumber,
                    Isvisible = l.Isvisible,
                    Createdat = l.Createdat
                }).ToList();
        }

        public async Task<PageLayerDto> GetByIdAsync(int id)
        {
            var l = await _pageLayerRepository.GetByIdAsync(id);
            if (l == null) return null;

            return new PageLayerDto
            {
                Layerid = l.Layerid,
                Pageid = l.Pageid,
                Uploaderid = l.Uploaderid,
                Layername = l.Layername,
                Fileurl = l.Fileurl,
                Zindex = l.Zindex,
                Versionnumber = l.Versionnumber,
                Isvisible = l.Isvisible,
                Createdat = l.Createdat
            };
        }

        public Task<int> CreateAsync(PageLayerDto.Create dto, string fileStorageUrl)
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
            return _pageLayerRepository.CreateAsync(layer);
        }
        public async Task<int> UpdateAsync(int id, PageLayerDto.Update pageLayerDto, string fileUrl)
        {
            var existing = await _pageLayerRepository.GetByIdAsync(id);
            if (existing == null) return 0;

            existing.Layername = pageLayerDto.Layername;
            existing.Zindex = pageLayerDto.Zindex;
            existing.Versionnumber = pageLayerDto.Versionnumber;
            existing.Isvisible = pageLayerDto.Isvisible;

            existing.Fileurl = fileUrl;

            return await _pageLayerRepository.UpdateAsync(existing);
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var existing = await _pageLayerRepository.GetByIdAsync(id);
            if (existing == null) return false;

            return await _pageLayerRepository.RemoveAsync(existing);
        }
    }
}
