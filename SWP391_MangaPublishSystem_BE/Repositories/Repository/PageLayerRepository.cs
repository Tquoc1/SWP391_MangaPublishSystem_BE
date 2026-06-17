using Entities.Models;
using Membership.Repositories.QuocDT.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class PageLayerRepository : GenericRepository<Pagelayer>
    {
        public PageLayerRepository() { }
        public PageLayerRepository(MangaPublishDBContext context) : base(context) => _context = context;

        public async Task<List<Pagelayer>> GetLayersAsync(int? pageId = null)
        {
            var query = _context.Pagelayers.AsQueryable();

            if (pageId.HasValue)
            {
                query = query.Where(l => l.Pageid == pageId.Value);
            }

            return await query.OrderBy(l => l.Zindex).ToListAsync();
        }

        public async Task<Pagelayer> GetLayerByIdAsync(int id)
        {
            return await _context.Pagelayers.FirstOrDefaultAsync(l => l.Layerid == id);
        }

        public async Task<List<Pagelayer>> GetByPageIdAsync(int pageId)
        {
            return await _context.Pagelayers
                .Where(l => l.Pageid == pageId)
                .OrderBy(l => l.Zindex)
                .ToListAsync();
        }
    }
}
