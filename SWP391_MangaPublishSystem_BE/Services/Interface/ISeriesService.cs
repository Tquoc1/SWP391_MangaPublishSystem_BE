using Entities.Models;
using DTOs;

namespace Services.Interface
{
    public interface ISeriesService
    {
        Task<List<SeriesDto>> GetAllAsync();
        Task<SeriesDto> GetByIdAsync(int id);
        Task<List<SeriesDto>> GetByMangakaIdAsync(int mangakaId);
        Task<int> CreateAsync(SeriesDto.Create seriesDto, string proposalFileUrl , string coverImageUrl);
        Task UpdateAsync(int id, SeriesDto.Update seriesDto, string proposalFileUrl, string coverImageUrl);
        Task UpdateStatusAsync(int id, SeriesDto.UpdateStatus seriesDto);
        Task UpdatePublishFormatAsync(int id, SeriesDto.UpdatePublishFormat seriesDto);
        Task SoftDeleteAsync(int id);
        Task RemoveAsync(int id);
        Task UpdateTantouEditorAsync(int id, int tantouEditorId);
        Task UploadCoverAsync(int id, string coverImageUrl);
        Task UploadProposalAsync(int id, string proposalFileUrl);
    }
}
