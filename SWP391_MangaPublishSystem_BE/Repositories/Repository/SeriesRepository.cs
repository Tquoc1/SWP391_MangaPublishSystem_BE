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

        public async Task<List<Series>> GetAllWithDetailsAsync(int? mangakaId = null, string? status = null)
        {
            var query = _context.Series.AsQueryable();

            query = query.Where(s => s.Isdeleted != true);

            if (mangakaId.HasValue)
            {
                query = query.Where(s => s.Mangakaid == mangakaId.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(s => s.Status == status);
            }

            return await query
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

        public new async Task<int> CreateAsync(Series entity)
        {
            if (entity.Genres != null && entity.Genres.Any())
            {
                foreach (var g in entity.Genres)
                {
                    _context.Attach(g);
                }
            }
            if (entity.Tags != null && entity.Tags.Any())
            {
                foreach (var t in entity.Tags)
                {
                    _context.Attach(t);
                }
            }
            _context.Series.Add(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateWithDetailsAsync(Series entity, List<int> genreIds, List<int> tagIds)
        {
            var existing = await _context.Series
                .AsTracking()
                .Include(s => s.Genres)
                .Include(s => s.Tags)
                .FirstOrDefaultAsync(s => s.Seriesid == entity.Seriesid);

            if (existing == null) return false;

            existing.Title = entity.Title;
            existing.Synopsis = entity.Synopsis;
            existing.Agerating = entity.Agerating;
            if (!string.IsNullOrEmpty(entity.Proposalfileurl)) existing.Proposalfileurl = entity.Proposalfileurl;
            if (!string.IsNullOrEmpty(entity.Coverimageurl)) existing.Coverimageurl = entity.Coverimageurl;

            if (genreIds != null)
            {
                existing.Genres.Clear();
                if (genreIds.Any())
                {
                    var genres = await _context.Genres.Where(g => genreIds.Contains(g.Genreid)).ToListAsync();
                    foreach (var g in genres) existing.Genres.Add(g);
                }
            }

            if (tagIds != null)
            {
                existing.Tags.Clear();
                if (tagIds.Any())
                {
                    var tags = await _context.Tags.Where(t => tagIds.Contains(t.Tagid)).ToListAsync();
                    foreach (var t in tags) existing.Tags.Add(t);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
