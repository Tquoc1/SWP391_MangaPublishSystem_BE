using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs;

namespace Services.Interface
{
    public interface IMangakaAssistantService
    {
        Task<List<MangakaAssistantDto>> GetAllAsync(int? mangakaId, int? assistantId);
        Task<MangakaAssistantDto> GetByIdAsync(int id);
        Task<int> CreateAsync(MangakaAssistantDto.Create dto);
        Task<int> UpdateAsync(int id, MangakaAssistantDto.Update dto);
        Task<int> UpdateStatusAsync(int id, string status);
        Task<bool> RemoveAsync(int id);
    }
}
