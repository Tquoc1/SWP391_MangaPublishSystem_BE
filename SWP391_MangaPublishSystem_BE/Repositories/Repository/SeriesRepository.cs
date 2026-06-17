using Entities.Models;
using Membership.Repositories.QuocDT.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class SeriesRepository : GenericRepository<Series>
    {

        private readonly MangaPublishDBContext _context;

        public SeriesRepository() { }

        public SeriesRepository(MangaPublishDBContext context) : base(context)
        {
            _context = context;
        }

        public new async Task<List<Series>> GetAllAsync()
        {
            return await _context.Series
                .Include(s => s.Genres) 
                .Include(s => s.Tags)   
                .ToListAsync();
        }


        public new async Task<Series?> GetByIdAsync(int id)
        {
            return await _context.Series
                .Include(s => s.Genres)
                .Include(s => s.Tags)
                .FirstOrDefaultAsync(s => s.Seriesid == id);
        }


        public new async Task<int> CreateAsync(Series series)
        {
            if (series.Genres != null && series.Genres.Any())
            {
                foreach (var genre in series.Genres)
                {
                    _context.Entry(genre).State = EntityState.Unchanged;
                }
            }

            if (series.Tags != null && series.Tags.Any())
            {
                foreach (var tag in series.Tags)
                {
                    _context.Entry(tag).State = EntityState.Unchanged;
                }
            }

            await _context.Series.AddAsync(series);

            await _context.SaveChangesAsync();

            return series.Seriesid;
        }
    }
}