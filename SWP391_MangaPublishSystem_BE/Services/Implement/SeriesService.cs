using Entities.Models;
using Services.DTO;
using Repositories.Repository;
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

        public async Task<List<SeriesDto>> GetByMangakaIdAsync(int mangakaId)
        {
            var series = await _seriesRepository.GetAllAsync();

            return series
                .Where(s => s.Mangakaid == mangakaId && s.Isdeleted != true)
                .Select(s => new SeriesDto
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

        public Task<int> CreateAsync(SeriesDto.Create seriesDto, string proposalFileUrl)
        {
            var series = new Series
            {
                Title = seriesDto.Title,
                Synopsis = seriesDto.Synopsis,
                Mangakaid = seriesDto.Mangakaid,
                Tantoueditorid = seriesDto.Tantoueditorid,
                Proposalfileurl = proposalFileUrl,
                Status = "Pending", // Trạng thái mặc định khi tạo mới
                Createdat = DateTime.UtcNow,
                Isdeleted = false
            };
            return _seriesRepository.CreateAsync(series);
        }

        public async Task<bool> UpdateAsync(int id, SeriesDto.Update seriesDto, string proposalFileUrl)
        {
            var existing = await _seriesRepository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.Title = seriesDto.Title;
            existing.Synopsis = seriesDto.Synopsis;

            if (!string.IsNullOrEmpty(proposalFileUrl))
            {
                existing.Proposalfileurl = proposalFileUrl;
            }

            var result = await _seriesRepository.UpdateAsync(existing);
            return result > 0; 
        }


        public async Task<bool> UpdateStatusAsync(int id, SeriesDto.UpdateStatus seriesDto)
        {
            var existing = await _seriesRepository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.Status = seriesDto.Status;

            if (seriesDto.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
            {
                existing.Approvedat = DateTime.UtcNow;
            }

            var result = await _seriesRepository.UpdateAsync(existing);
            return result > 0;
        }

        public async Task<bool> UpdatePublishFormatAsync(int id, SeriesDto.UpdatePublishFormat seriesDto)
        {
            var existing = await _seriesRepository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.Publishformat = seriesDto.Publishformat;

            var result = await _seriesRepository.UpdateAsync(existing);
            return result > 0;
        }

 
        public async Task<bool> SoftDeleteAsync(int id)
        {
            var existing = await _seriesRepository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.Isdeleted = true; // Bật cờ xóa mềm

            var result = await _seriesRepository.UpdateAsync(existing);
            return result > 0;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var existing = await _seriesRepository.GetByIdAsync(id);
            if (existing == null) return false;

            return await _seriesRepository.RemoveAsync(existing);
        }
    }
}