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
    public class SeriesRepository : GenericRepository<Series>
    {
        private readonly MangaPublishDBContext _context;

        public SeriesRepository() { }

        public SeriesRepository(MangaPublishDBContext context) : base(context)
        {
            _context = context;
        }

        // Get all series with genres and tags
        public async Task<List<SeriesDto>> GetAllAsync()
        {
            var seriesList = await _context.Series
                .Include(s => s.Genres) 
                .Include(s => s.Tags)   
                .ToListAsync();

            return seriesList.Select(MapToDto).ToList();
        }

        // Get series by id with genres and tags
        public async Task<SeriesDto> GetByIdAsync(int id)
        {
            var series = await _context.Series
                .Include(s => s.Genres)
                .Include(s => s.Tags)
                .FirstOrDefaultAsync(s => s.Seriesid == id);

            return series != null ? MapToDto(series) : null;
        }

        // Get series by mangaka id
        public async Task<List<SeriesDto>> GetByMangakaIdAsync(int mangakaId)
        {
            var seriesList = await _context.Series
                .Include(s => s.Genres)
                .Include(s => s.Tags)
                .Where(s => s.Mangakaid == mangakaId && s.Isdeleted != true)
                .ToListAsync();

            return seriesList.Select(MapToDto).ToList();
        }

        // Create series 
        public async Task<int> CreateWithDetailsAsync(SeriesDto.Create seriesDto, string proposalFileUrl, string coverImageUrl)
        {
            var series = new Series
            {
                Title = seriesDto.Title,
                Synopsis = seriesDto.Synopsis,
                Mangakaid = seriesDto.Mangakaid,
                Agerating = seriesDto.Agerating ?? "G",
                Tantoueditorid = seriesDto.Tantoueditorid,
                Publishformat = "Pending",
                Proposalfileurl = proposalFileUrl,
                Coverimageurl = coverImageUrl,
                Status = "Pending",
                Createdat = DateTime.UtcNow,
                Isdeleted = false
            };

            if (seriesDto.GenreIds != null && seriesDto.GenreIds.Any())
            {
                series.Genres = new List<Genre>();
                foreach (var gid in seriesDto.GenreIds)
                {
                    var genre = new Genre { Genreid = gid };
                    _context.Entry(genre).State = EntityState.Unchanged; 
                    series.Genres.Add(genre);
                }
            }

            if (seriesDto.TagIds != null && seriesDto.TagIds.Any())
            {
                series.Tags = new List<Tag>();
                foreach (var tid in seriesDto.TagIds)
                {
                    var tag = new Tag { Tagid = tid };
                    _context.Entry(tag).State = EntityState.Unchanged; 
                    series.Tags.Add(tag);
                }
            }

            await _context.Series.AddAsync(series);
            await _context.SaveChangesAsync();

            return series.Seriesid;
        }

        // Update series for mangaka 
        public async Task<int> UpdateSeriesAsync(int id, SeriesDto.Update seriesDto, string proposalFileUrl, string coverImageUrl)
        {
            var existing = await _context.Series
                .Include(s => s.Genres)
                .Include(s => s.Tags)
                .FirstOrDefaultAsync(s => s.Seriesid == id);

            if (existing == null) return 0;

            if (!string.IsNullOrEmpty(seriesDto.Title)) existing.Title = seriesDto.Title;
            if (!string.IsNullOrEmpty(seriesDto.Synopsis)) existing.Synopsis = seriesDto.Synopsis;
            if (!string.IsNullOrEmpty(seriesDto.Agerating)) existing.Agerating = seriesDto.Agerating;

            if (!string.IsNullOrEmpty(proposalFileUrl)) existing.Proposalfileurl = proposalFileUrl;
            if (!string.IsNullOrEmpty(coverImageUrl)) existing.Coverimageurl = coverImageUrl;


            if (seriesDto.GenreIds != null)
            {
                existing.Genres.Clear();
                var genresToAdd = await _context.Genres
                    .Where(g => seriesDto.GenreIds.Contains(g.Genreid))
                    .ToListAsync();

                foreach (var genre in genresToAdd)
                {
                    existing.Genres.Add(genre);
                }
            }

 
            if (seriesDto.TagIds != null)
            {
                existing.Tags.Clear();
                var tagsToAdd = await _context.Tags
                    .Where(t => seriesDto.TagIds.Contains(t.Tagid))
                    .ToListAsync();

                foreach (var tag in tagsToAdd)
                {
                    existing.Tags.Add(tag);
                }
            }

            return await _context.SaveChangesAsync();
        }

        // Update status
        public async Task<int> UpdateStatusAsync(int id, string status)
        {
            var existing = await _context.Series.FindAsync(id);
            if (existing == null) return 0;

            existing.Status = status;
            if (status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
            {
                existing.Approvedat = DateTime.UtcNow;
            }

            _context.ChangeTracker.Clear();
            var tracker = _context.Attach(existing);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        // Update publish format
        public async Task<int> UpdatePublishFormatAsync(int id, string publishformat)
        {
            var existing = await _context.Series.FindAsync(id);
            if (existing == null) return 0;

            existing.Publishformat = publishformat;

            _context.ChangeTracker.Clear();
            var tracker = _context.Attach(existing);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        // Soft delete
        public async Task<int> SoftDeleteAsync(int id)
        {
            var existing = await _context.Series.FindAsync(id);
            if (existing == null) return 0;

            existing.Isdeleted = true;

            _context.ChangeTracker.Clear();
            var tracker = _context.Attach(existing);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        // Hard delete
        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Series.FindAsync(id);
            if (existing == null) return false;

            _context.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        // Map entity to DTO
        private SeriesDto MapToDto(Series series)
        {
            return new SeriesDto
            {
                Seriesid = series.Seriesid,
                Title = series.Title,
                Synopsis = series.Synopsis,
                Mangakaid = series.Mangakaid,
                Tantoueditorid = series.Tantoueditorid,
                Publishformat = series.Publishformat,
                Status = series.Status,
                Proposalfileurl = series.Proposalfileurl,
                Coverimageurl = series.Coverimageurl,
                Agerating = series.Agerating,
                Createdat = series.Createdat,
                Approvedat = series.Approvedat,
                Isdeleted = series.Isdeleted,
                Genres = series.Genres?.Select(g => new SeriesDto.GenreSimpleDto
                {
                    GenreId = g.Genreid,
                    GenreName = g.Genrename
                }).ToList() ?? new List<SeriesDto.GenreSimpleDto>(),
                Tags = series.Tags?.Select(t => new SeriesDto.TagSimpleDto
                {
                    TagId = t.Tagid,
                    TagName = t.Tagname
                }).ToList() ?? new List<SeriesDto.TagSimpleDto>()
            };
        }
    }

}
