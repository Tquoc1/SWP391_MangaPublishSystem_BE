using Entities.Models;
using Services.DTO;
using Repositories.Repository;
using Services.Interface;

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
            var series = await _seriesRepository.GetAllAsync();
            return series.Select(s => new SeriesDto
            {
                Seriesid = s.Seriesid,
                Title = s.Title,
                Synopsis = s.Synopsis,
                Mangakaid = s.Mangakaid,
                Tantoueditorid = s.Tantoueditorid,
                Publishformat = s.Publishformat,
                Status = s.Status,
                Proposalfileurl = s.Proposalfileurl,
                Createdat = s.Createdat,
                Approvedat = s.Approvedat,
                Isdeleted = s.Isdeleted
            }).ToList();
        }

        public async Task<SeriesDto> GetByIdAsync(int id)
        {
            var s = await _seriesRepository.GetByIdAsync(id);
            if (s == null) return null;

            return new SeriesDto
            {
                Seriesid = s.Seriesid,
                Title = s.Title,
                Synopsis = s.Synopsis,
                Mangakaid = s.Mangakaid,
                Tantoueditorid = s.Tantoueditorid,
                Publishformat = s.Publishformat,
                Status = s.Status,
                Proposalfileurl = s.Proposalfileurl,
                Createdat = s.Createdat,
                Approvedat = s.Approvedat,
                Isdeleted = s.Isdeleted
            };
        }

        public Task<int> CreateAsync(SeriesDto.Create seriesDto, string proposalFileUrl)
        {
            var series = new Series
            {
                Title = seriesDto.Title,
                Synopsis = seriesDto.Synopsis,
                Mangakaid = seriesDto.Mangakaid,
                Tantoueditorid = seriesDto.Tantoueditorid,
                //Publishformat = seriesDto.Publishformat,
                Proposalfileurl = proposalFileUrl,
                Status = "Pending", // Default value
                Createdat = DateTime.UtcNow,
                Isdeleted = false
            };
            return _seriesRepository.CreateAsync(series);
        }

        public async Task<int> UpdateAsync(int id, SeriesDto.Update seriesDto, string proposalFileUrl)
        {
            var existing = await _seriesRepository.GetByIdAsync(id);
            if (existing == null) return 0;

            existing.Title = seriesDto.Title;
            existing.Synopsis = seriesDto.Synopsis;
            existing.Publishformat = seriesDto.Publishformat;
            existing.Status = seriesDto.Status;
            //existing.Proposalfileurl = seriesDto.Proposalfileurl;

            if (seriesDto.Isdeleted.HasValue)
            {
                existing.Isdeleted = seriesDto.Isdeleted;
            }

            return await _seriesRepository.UpdateAsync(existing);
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var existing = await _seriesRepository.GetByIdAsync(id);
            if (existing == null) return false;

            return await _seriesRepository.RemoveAsync(existing);
        }
    }
}
