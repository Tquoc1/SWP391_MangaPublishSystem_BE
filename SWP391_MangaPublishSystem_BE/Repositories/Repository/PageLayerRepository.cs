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

        public async Task<List<Pagelayer>> GetLayersAsync(int? pageId = null, bool includeDeleted = false)
        {
            var query = _context.Pagelayers.AsQueryable();

            if (!includeDeleted)
            {
                query = query.Where(l => l.Isdeleted == false);
            }

            if (pageId.HasValue)
            {
                query = query.Where(l => l.Pageid == pageId.Value);
            }

            return await query.OrderBy(l => l.Zindex).ToListAsync();
        }

        public async Task<Pagelayer> GetLayerByIdAsync(int id, bool includeDeleted = false)
        {
            var query = _context.Pagelayers.AsQueryable();
            if (!includeDeleted)
            {
                query = query.Where(l => l.Isdeleted == false);
            }
            return await query.FirstOrDefaultAsync(l => l.Layerid == id);
        }

        public async Task<List<Pagelayer>> GetByPageIdAsync(int pageId, bool includeDeleted = false)
        {
            var query = _context.Pagelayers.AsQueryable();
            if (!includeDeleted)
            {
                query = query.Where(l => l.Isdeleted == false);
            }
            return await query
                .Where(l => l.Pageid == pageId)
                .OrderBy(l => l.Zindex)
                .ToListAsync();
        }
    }
}
