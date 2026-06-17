using Entities.Models;
using Membership.Repositories.QuocDT.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class SeriesRepository : GenericRepository<Series>
    {
        public SeriesRepository() { }

        public SeriesRepository(MangaPublishDBContext context) : base(context)
        {
        }

        public async Task<List<Series>> GetAllWithDetailsAsync()
        {
            return await _context.Series
                .Include(s => s.Genres)
                .Include(s => s.Tags)
                .ToListAsync();
        }

        public async Task<Series> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Series
                .Include(s => s.Genres)
                .Include(s => s.Tags)
                .FirstOrDefaultAsync(s => s.Seriesid == id);
        }

        public async Task<List<Series>> GetByMangakaIdAsync(int mangakaId)
        {
            return await _context.Series
                .Include(s => s.Genres)
                .Include(s => s.Tags)
                .Where(s => s.Mangakaid == mangakaId && s.Isdeleted != true)
                .ToListAsync();
        }

        public async Task<List<Genre>> GetGenresByIdsAsync(List<int> genreIds)
        {
            return await _context.Genres.Where(g => genreIds.Contains(g.Genreid)).ToListAsync();
        }

        public async Task<List<Tag>> GetTagsByIdsAsync(List<int> tagIds)
        {
            return await _context.Tags.Where(t => tagIds.Contains(t.Tagid)).ToListAsync();
        }
    }
}
