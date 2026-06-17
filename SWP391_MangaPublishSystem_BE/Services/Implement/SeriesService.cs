using Repositories.Repository;
using DTOs;
using Services.Interface;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
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
            var seriesList = await _seriesRepository.GetAllWithDetailsAsync();
            return seriesList.Select(MapToDto).ToList();
        }

        public async Task<SeriesDto> GetByIdAsync(int id)
        {
            var series = await _seriesRepository.GetByIdWithDetailsAsync(id);
            return series != null ? MapToDto(series) : null;
        }

        public async Task<List<SeriesDto>> GetByMangakaIdAsync(int mangakaId)
        {
            var seriesList = await _seriesRepository.GetByMangakaIdAsync(mangakaId);
            return seriesList.Select(MapToDto).ToList();
        }

        public async Task<int> CreateAsync(SeriesDto.Create seriesDto, string proposalFileUrl, string coverImageUrl)
        {
            var series = new Series
            {
                Title = seriesDto.Title,
                Synopsis = seriesDto.Synopsis,
                Mangakaid = seriesDto.Mangakaid,
                Agerating = seriesDto.Agerating ?? "G",
                Tantoueditorid = seriesDto.Tantoueditorid,
                Publishformat = "Pending",
                Proposalfileurl = proposalFileUrl,
                Coverimageurl = coverImageUrl,
                Status = "Pending",
                Createdat = DateTime.UtcNow,
                Isdeleted = false
            };

            if (seriesDto.GenreIds != null && seriesDto.GenreIds.Any())
            {
                var genres = await _seriesRepository.GetGenresByIdsAsync(seriesDto.GenreIds);
                series.Genres = genres;
            }

            if (seriesDto.TagIds != null && seriesDto.TagIds.Any())
            {
                var tags = await _seriesRepository.GetTagsByIdsAsync(seriesDto.TagIds);
                series.Tags = tags;
            }

            await _seriesRepository.CreateAsync(series);
            return series.Seriesid;
        }

        public async Task<bool> UpdateAsync(int id, SeriesDto.Update seriesDto, string proposalFileUrl, string coverImageUrl)
        {
            var existing = await _seriesRepository.GetByIdWithDetailsAsync(id);
            if (existing == null) return false;

            if (!string.IsNullOrEmpty(seriesDto.Title)) existing.Title = seriesDto.Title;
            if (!string.IsNullOrEmpty(seriesDto.Synopsis)) existing.Synopsis = seriesDto.Synopsis;
            if (!string.IsNullOrEmpty(seriesDto.Agerating)) existing.Agerating = seriesDto.Agerating;
            if (!string.IsNullOrEmpty(proposalFileUrl)) existing.Proposalfileurl = proposalFileUrl;
            if (!string.IsNullOrEmpty(coverImageUrl)) existing.Coverimageurl = coverImageUrl;

            if (seriesDto.GenreIds != null)
            {
                existing.Genres.Clear();
                var genres = await _seriesRepository.GetGenresByIdsAsync(seriesDto.GenreIds);
                foreach (var genre in genres) existing.Genres.Add(genre);
            }

            if (seriesDto.TagIds != null)
            {
                existing.Tags.Clear();
                var tags = await _seriesRepository.GetTagsByIdsAsync(seriesDto.TagIds);
                foreach (var tag in tags) existing.Tags.Add(tag);
            }

            await _seriesRepository.UpdateAsync(existing);
            return true;
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

            await _seriesRepository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> UpdatePublishFormatAsync(int id, SeriesDto.UpdatePublishFormat seriesDto)
        {
            var existing = await _seriesRepository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.Publishformat = seriesDto.Publishformat;
            await _seriesRepository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var existing = await _seriesRepository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.Isdeleted = true;
            await _seriesRepository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var existing = await _seriesRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _seriesRepository.RemoveAsync(existing);
            return true;
        }

        private SeriesDto MapToDto(Series series)
        {
            return new SeriesDto
            {
                Seriesid = series.Seriesid,
                Title = series.Title,
                Synopsis = series.Synopsis,
                Mangakaid = series.Mangakaid,
                Tantoueditorid = series.Tantoueditorid,
                Publishformat = series.Publishformat,
                Status = series.Status,
                Proposalfileurl = series.Proposalfileurl,
                Coverimageurl = series.Coverimageurl,
                Agerating = series.Agerating,
                Createdat = series.Createdat,
                Approvedat = series.Approvedat,
                Isdeleted = series.Isdeleted,
                Genres = series.Genres?.Select(g => new SeriesDto.GenreSimpleDto
                {
                    GenreId = g.Genreid,
                    GenreName = g.Genrename
                }).ToList() ?? new List<SeriesDto.GenreSimpleDto>(),
                Tags = series.Tags?.Select(t => new SeriesDto.TagSimpleDto
                {
                    TagId = t.Tagid,
                    TagName = t.Tagname
                }).ToList() ?? new List<SeriesDto.TagSimpleDto>()
            };
        }
    }
}
