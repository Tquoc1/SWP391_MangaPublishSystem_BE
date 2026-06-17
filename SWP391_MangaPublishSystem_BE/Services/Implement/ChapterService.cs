using Repositories.Repository;
using DTOs;
using Services.Interface;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var chapters = await _chapterRepository.GetChaptersAsync(seriesId);
            return chapters.Select(MapToDto).ToList();
        }

        public async Task<ChapterDto> GetByIdAsync(int id)
        {
            var chapter = await _chapterRepository.GetChapterByIdAsync(id);
            return chapter != null ? MapToDto(chapter) : null;
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

            await _chapterRepository.UpdateAsync(existing);
            return 1;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var existing = await _chapterRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _chapterRepository.RemoveAsync(existing);
            return true;
        }

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
