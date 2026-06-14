using Entities.Models;
using Services.DTO;

namespace Services.Interface
{
    public interface ISeriesService
    {
        Task<List<SeriesDto>> GetAllAsync();
        Task<SeriesDto> GetByIdAsync(int id);
        Task<List<SeriesDto>> GetByMangakaIdAsync(int mangakaId);
        Task<int> CreateAsync(SeriesDto.Create seriesDto, string proposalFileUrl);
        Task<bool> UpdateAsync(int id, SeriesDto.Update seriesDto, string proposalFileUrl);
        Task<bool> UpdateStatusAsync(int id, SeriesDto.UpdateStatus seriesDto);
        Task<bool> UpdatePublishFormatAsync(int id, SeriesDto.UpdatePublishFormat seriesDto);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> RemoveAsync(int id);
    }
}
