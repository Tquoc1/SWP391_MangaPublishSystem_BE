using Entities.Models;
using Services.DTO;

namespace Services.Interface
{
    public interface ISeriesService
    {
        Task<List<SeriesDto>> GetAllAsync();
        Task<SeriesDto> GetByIdAsync(int id);
        Task<int> CreateAsync(SeriesDto.Create seriesDto, string proposalFileUrl);
        Task<int> UpdateAsync(int id, SeriesDto.Update seriesDto, string proposalFileUrl);
        Task<bool> RemoveAsync(int id);
    }
}
