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
        private static readonly Dictionary<string, List<string>> _validTransitions = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Drafting", new List<string> { "StudioWorking", "Cancelled" } },
            { "StudioWorking", new List<string> { "EditorReviewing", "Cancelled", "Delayed" } },
            { "EditorReviewing", new List<string> { "ReadyForPrint", "StudioWorking", "Cancelled", "Delayed" } },
            { "ReadyForPrint", new List<string> { "Published", "Delayed" } },
            { "Published", new List<string> { "Archived" } },
            { "Delayed", new List<string> { "StudioWorking", "EditorReviewing", "ReadyForPrint" } }
        };

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

        public async Task<List<ChapterDto>> GetByAssistantIdAsync(int assistantId)
        {
            var chapters = await _chapterRepository.GetChaptersByAssistantIdAsync(assistantId);
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

        public async Task UpdateAsync(ChapterDto.Update chapterDto)
        {
            var existing = await _chapterRepository.GetByIdAsync(chapterDto.Chapterid);
            if (existing == null) throw new KeyNotFoundException("Không tìm thấy chương truyện cần cập nhật.");

            existing.Chapternumber = chapterDto.Chapternumber;
            existing.Title = chapterDto.Title;
            existing.Deadline = chapterDto.Deadline;

            await _chapterRepository.UpdateAsync(existing);
        }

        public async Task UpdateStatusAsync(int id, string status)
        {
            var existing = await _chapterRepository.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Không tìm thấy chương truyện để cập nhật trạng thái.");

            if (string.Equals(existing.Status, status, StringComparison.OrdinalIgnoreCase))
                return;

            if (!_validTransitions.ContainsKey(existing.Status) || 
                !_validTransitions[existing.Status].Contains(status, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Không thể chuyển trạng thái từ '{existing.Status}' sang '{status}'. Luồng không hợp lệ!");
            }

            existing.Status = status;
            await _chapterRepository.UpdateAsync(existing);
        }

        public async Task SoftDeleteAsync(int id)
        {
            var existing = await _chapterRepository.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Không tìm thấy chương truyện để xóa tạm.");

            existing.Isdeleted = true;
            await _chapterRepository.UpdateAsync(existing);
        }

        public async Task RemoveAsync(int id)
        {
            var existing = await _chapterRepository.GetByIdAsync(id);

            if (existing == null || existing.Isdeleted == true)
                throw new KeyNotFoundException("Không tìm thấy chương truyện.");

            existing.Isdeleted = true;

            await _chapterRepository.UpdateAsync(existing);
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
