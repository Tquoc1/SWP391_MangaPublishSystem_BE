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
    public class ChapterRepository : GenericRepository<Chapter>
    {
        public ChapterRepository() { }
        public ChapterRepository(MangaPublishDBContext context) : base(context) => _context = context;

        // Get all active chapters
        public async Task<List<ChapterDto>> GetAllActiveAsync(int? seriesId = null)
        {
            var query = _context.Chapters.Where(c => c.Isdeleted == false);

            if (seriesId.HasValue)
            {
                query = query.Where(c => c.Seriesid == seriesId.Value);
            }

            var chapters = await query.ToListAsync();
            return chapters.Select(MapToDto).ToList();
        }

        // Get active chapter by id
        public async Task<ChapterDto> GetByIdActiveAsync(int id)
        {
            var chapter = await _context.Chapters
                .FirstOrDefaultAsync(c => c.Chapterid == id && c.Isdeleted == false);
            return chapter != null ? MapToDto(chapter) : null;
        }

        // Get chapters by series id
        public async Task<List<Chapter>> GetBySeriesIdAsync(int seriesId)
        {
            return await _context.Chapters
                .Where(c => c.Seriesid == seriesId && c.Isdeleted == false)
                .ToListAsync();
        }

        // Create chapter with default values
        public async Task<int> CreateWithDefaultsAsync(int seriesId, int chapterNumber, string title, DateTime deadline)
        {
            var chapter = new Chapter
            {
                Seriesid = seriesId,
                Chapternumber = chapterNumber,
                Title = title,
                Deadline = deadline,
                Status = "Drafting",
                Createdat = DateTime.UtcNow,
                Isdeleted = false
            };

            _context.Chapters.Add(chapter);
            await _context.SaveChangesAsync();
            return chapter.Chapterid;
        }

        // Update chapter
        public async Task<int> UpdateChapterAsync(int chapterId, int chapterNumber, string title, DateTime deadline)
        {
            var existing = await _context.Chapters.FindAsync(chapterId);
            if (existing == null) return 0;

            existing.Chapternumber = chapterNumber;
            existing.Title = title;
            existing.Deadline = deadline;
            //existing.Status = status;

            //if (isDeleted.HasValue)
            //{
            //    existing.Isdeleted = isDeleted;
            //}

            _context.ChangeTracker.Clear();
            var tracker = _context.Attach(existing);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        // Delete chapter (hard delete)
        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Chapters.FindAsync(id);
            if (existing == null) return false;

            _context.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        // Map entity to DTO
        private ChapterDto MapToDto(Chapter chapter)
        {
            return new ChapterDto
            {
                Chapterid = chapter.Chapterid,
                Seriesid = chapter.Seriesid,
                Chapternumber = chapter.Chapternumber,
                Title = chapter.Title,
                Deadline = chapter.Deadline,
                Status = chapter.Status,
                Createdat = chapter.Createdat,
                Isdeleted = chapter.Isdeleted
            };
        }
    }
}
