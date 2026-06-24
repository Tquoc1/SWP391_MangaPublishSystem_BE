using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IChapterService
    {
        Task<List<ChapterDto>> GetAllAsync(int? seriesId, string? status);
        Task<List<ChapterDto>> GetByAssistantIdAsync(int assistantId);
        Task<ChapterDto> GetByIdAsync(int id);
        Task<int> CreateAsync(ChapterDto.Create chapterDto);
        Task UpdateAsync(ChapterDto.Update chapterDto);
        Task UpdateStatusAsync(int id, string status);
        Task SoftDeleteAsync(int id);
        Task RemoveAsync(int id);
    }
}
