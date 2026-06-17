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
    public class PageLayerRepository : GenericRepository<Pagelayer>
    {
        public PageLayerRepository() { }
        public PageLayerRepository(MangaPublishDBContext context) : base(context) => _context = context;

        // Get all layers, optionally filtered by page
        public async Task<List<PageLayerDto>> GetAllAsync(int? pageId = null)
        {
            var query = _context.Pagelayers.AsQueryable();

            if (pageId.HasValue)
            {
                query = query.Where(l => l.Pageid == pageId.Value);
            }

            var layers = await query.OrderBy(l => l.Zindex).ToListAsync();
            return layers.Select(MapToDto).ToList();
        }

        // Get layer by id
        public async Task<PageLayerDto> GetByIdAsync(int id)
        {
            var layer = await _context.Pagelayers.FirstOrDefaultAsync(l => l.Layerid == id);
            return layer != null ? MapToDto(layer) : null;
        }

        // Get layers by page id
        public async Task<List<PageLayerDto>> GetByPageIdAsync(int pageId)
        {
            var layers = await _context.Pagelayers
                .Where(l => l.Pageid == pageId)
                .OrderBy(l => l.Zindex)
                .ToListAsync();

            return layers.Select(MapToDto).ToList();
        }

        // Create layer with default values
        public async Task<int> CreateWithDefaultsAsync(PageLayerDto.Create dto, string fileStorageUrl)
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

            _context.Pagelayers.Add(layer);
            await _context.SaveChangesAsync();
            return layer.Layerid;
        }

        // Update layer
        public async Task<int> UpdateLayerAsync(int id, PageLayerDto.Update pageLayerDto, string fileUrl)
        {
            var existing = await _context.Pagelayers.FindAsync(id);
            if (existing == null) return 0;

            existing.Layername = pageLayerDto.Layername;
            existing.Zindex = pageLayerDto.Zindex;
            existing.Versionnumber = pageLayerDto.Versionnumber;
            existing.Isvisible = pageLayerDto.Isvisible;
            existing.Fileurl = fileUrl;

            _context.ChangeTracker.Clear();
            var tracker = _context.Attach(existing);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        // Delete layer
        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Pagelayers.FindAsync(id);
            if (existing == null) return false;

            _context.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        // Map entity to DTO
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
