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
        Task<List<ChapterDto>> GetAllAsync(int? seriesId);
        Task<List<ChapterDto>> GetByAssistantIdAsync(int assistantId);
        Task<ChapterDto> GetByIdAsync(int id);
        Task<int> CreateAsync(ChapterDto.Create chapterDto);
        Task<int> UpdateAsync(ChapterDto.Update chapterDto);
        Task<bool> UpdateStatusAsync(int id, string status);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> RemoveAsync(int id);
    }
}
