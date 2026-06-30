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
            { "InProduction", new List<string> { "Ready", "Delayed", "Cancelled" } },
            { "Ready", new List<string> { "Published", "Delayed", "Cancelled" } },
            { "Published", new List<string> { "Delayed", "Cancelled" } },
            { "Delayed", new List<string> { "InProduction", "Ready", "Published", "Cancelled" } }
        };

        private readonly ChapterRepository _chapterRepository;
        private readonly SeriesRepository _seriesRepository;
        private readonly INotificationService _notificationService;

        public ChapterService(
            ChapterRepository chapterRepository,
            SeriesRepository seriesRepository,
            INotificationService notificationService)
        {
            _chapterRepository = chapterRepository;
            _seriesRepository = seriesRepository;
            _notificationService = notificationService;
        }

        public async Task<List<ChapterDto>> GetAllAsync(int? seriesId, string? status)
        {
            var chapters = await _chapterRepository.GetChaptersAsync(seriesId, status);
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
                Status = "InProduction",
                Createdat = DateTime.UtcNow,
                Isdeleted = false
            };

            await _chapterRepository.CreateAsync(chapter);

            var series = await _seriesRepository.GetByIdAsync(chapterDto.Seriesid);
            if (series != null && series.Mangakaid > 0)
            {
                await _notificationService.CreateNotificationAsync(
                    series.Mangakaid,
                    "Chương mới được tạo",
                    $"Chương {chapterDto.Chapternumber}: '{chapterDto.Title}' của tác phẩm '{series.Title}' đã được tạo.",
                    series.Seriesid,
                    "Chapter",
                    chapter.Chapterid
                );
            }

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

            var series = await _seriesRepository.GetByIdAsync(existing.Seriesid);
            if (series != null && series.Mangakaid > 0)
            {
                string statusVn = status == "Published" ? "đã phát hành" : (status == "Ready" ? "sẵn sàng xuất bản" : (status == "Delayed" ? "bị hoãn" : status));
                await _notificationService.CreateNotificationAsync(
                    series.Mangakaid,
                    "Cập nhật trạng thái Chương",
                    $"Chương {existing.Chapternumber} của tác phẩm '{series.Title}' đã chuyển sang trạng thái: {statusVn}.",
                    series.Seriesid,
                    "Chapter",
                    existing.Chapterid
                );
            }
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
