using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IPageService
    {
        Task<List<PageDto>> GetAllAsync(int? chapterId, bool? isSentToMangaka, int? mangakaId = null);
        Task<PageDto> GetByIdAsync(int id);
        Task<int> CreateAsync(PageDto.Create pageDto, string pageImageUrl);
        Task UpdateAsync(int id,PageDto.Update pageDto);
        Task<string> CompositeAndSaveImageAsync(int id);
        Task UpdateIsSentToMangakaAsync(int id, bool isSentToMangaka);
        Task SoftDeleteAsync(int id);
        Task RemoveAsync(int id);
        //Task<bool> UpdateStatusAsync(int id, PageDto.Update dto);
        //Task<bool> UploadImageAsync(int id, string pageImageUrl);
    }
}
