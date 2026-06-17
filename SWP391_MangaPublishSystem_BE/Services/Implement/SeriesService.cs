using Repositories.Repository;
using DTOs;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class SeriesService : ISeriesService
    {
        private readonly SeriesRepository _seriesRepository;

        public SeriesService(SeriesRepository seriesRepository)
        {
            _seriesRepository = seriesRepository;
        }

        public async Task<List<SeriesDto>> GetAllAsync()
        {
            return await _seriesRepository.GetAllAsync();
        }

        public async Task<SeriesDto> GetByIdAsync(int id)
        {
            return await _seriesRepository.GetByIdAsync(id);
        }

        public async Task<List<SeriesDto>> GetByMangakaIdAsync(int mangakaId)
        {
            return await _seriesRepository.GetByMangakaIdAsync(mangakaId);
        }

        public async Task<int> CreateAsync(SeriesDto.Create seriesDto, string proposalFileUrl, string coverImageUrl)
        {
            return await _seriesRepository.CreateWithDetailsAsync(seriesDto, proposalFileUrl, coverImageUrl);
        }

        public async Task<bool> UpdateAsync(int id, SeriesDto.Update seriesDto, string proposalFileUrl, string coverImageUrl)
        {
            var result = await _seriesRepository.UpdateSeriesAsync(id, seriesDto, proposalFileUrl, coverImageUrl);
            return result > 0;
        }

        public async Task<bool> UpdateStatusAsync(int id, SeriesDto.UpdateStatus seriesDto)
        {
            var result = await _seriesRepository.UpdateStatusAsync(id, seriesDto.Status);
            return result > 0;
        }

        public async Task<bool> UpdatePublishFormatAsync(int id, SeriesDto.UpdatePublishFormat seriesDto)
        {
            var result = await _seriesRepository.UpdatePublishFormatAsync(id, seriesDto.Publishformat);
            return result > 0;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var result = await _seriesRepository.SoftDeleteAsync(id);
            return result > 0;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            return await _seriesRepository.DeleteAsync(id);
        }
    }

}
