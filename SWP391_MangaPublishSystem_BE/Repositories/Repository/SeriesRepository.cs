using Entities.Models;
using Membership.Repositories.QuocDT.Base;

namespace Repositories.Repository
{
    public class SeriesRepository : GenericRepository<Series>
    {
        public SeriesRepository() { }
        public SeriesRepository(MangaPublishDBContext context) : base(context) => _context = context;
    }
}
