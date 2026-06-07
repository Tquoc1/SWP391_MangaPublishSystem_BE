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
    public class ChapterService : IChapterService
    {
        private readonly ChapterRepository _chapterRepository;

        public ChapterService(ChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }

        public async Task<List<ChapterDto>> GetAllAsync(int? seriesId = null)
        {
            var chapters = await _chapterRepository.GetAllAsync();

            // Nếu có truyền seriesId -> Lọc theo bộ truyện. Nếu không -> Lấy hết.
            var query = chapters.Where(c => c.Isdeleted == false);
            if (seriesId.HasValue)
            {
                query = query.Where(c => c.Seriesid == seriesId.Value);
            }

            return query.Select(c => new ChapterDto
            {
                Chapterid = c.Chapterid,
                Seriesid = c.Seriesid,
                Chapternumber = c.Chapternumber,
                Title = c.Title,
                Deadline = c.Deadline,
                Status = c.Status,
                Createdat = c.Createdat,
                Isdeleted = c.Isdeleted
            }).ToList();
        }

        public async Task<ChapterDto> GetByIdAsync(int id)
        {
            var c = await _chapterRepository.GetByIdAsync(id);
            if (c == null || c.Isdeleted == true) return null;

            return new ChapterDto
            {
                Chapterid = c.Chapterid,
                Seriesid = c.Seriesid,
                Chapternumber = c.Chapternumber,
                Title = c.Title,
                Deadline = c.Deadline,
                Status = c.Status,
                Createdat = c.Createdat,
                Isdeleted = c.Isdeleted
            };
        }

        public async Task<int> CreateAsync(ChapterDto.Create chapterDto)
        {
            var chapter = new Chapter
            {
                Seriesid = chapterDto.Seriesid,
                Chapternumber = chapterDto.Chapternumber,
                Title = chapterDto.Title,
                Deadline = chapterDto.Deadline,
                Status = "Drafting", 
                Createdat = DateTime.UtcNow,
                Isdeleted = false
            };

            await _chapterRepository.CreateAsync(chapter);

            return chapter.Chapterid; 
        }

        public async Task<int> UpdateAsync(ChapterDto.Update chapterDto)
        {
            var existing = await _chapterRepository.GetByIdAsync(chapterDto.Chapterid);
            if (existing == null) return 0;

            existing.Chapternumber = chapterDto.Chapternumber;
            existing.Title = chapterDto.Title;
            existing.Deadline = chapterDto.Deadline;
            existing.Status = chapterDto.Status; 

            if (chapterDto.Isdeleted.HasValue)
            {
                existing.Isdeleted = chapterDto.Isdeleted;
            }

            return await _chapterRepository.UpdateAsync(existing);
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var existing = await _chapterRepository.GetByIdAsync(id);
            if (existing == null) return false;
            return await _chapterRepository.RemoveAsync(existing);
        }
    }
}
