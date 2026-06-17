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
        private readonly GenreRepository _genreRepository;
        private readonly TagRepository _tagRepository;

        public SeriesService(
            SeriesRepository seriesRepository,
            GenreRepository genreRepository,
            TagRepository tagRepository)
        {
            _seriesRepository = seriesRepository;
            _genreRepository = genreRepository;
            _tagRepository = tagRepository;
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
                Coverimageurl = s.Coverimageurl,
                Agerating = s.Agerating,
                Createdat = s.Createdat,
                Approvedat = s.Approvedat,
                Isdeleted = s.Isdeleted,
                // EF Core map trực tiếp qua s.Genres
                Genres = s.Genres?.Select(g => new SeriesDto.GenreSimpleDto
                {
                    GenreId = g.Genreid,
                    GenreName = g.Genrename
                }).ToList() ?? new List<SeriesDto.GenreSimpleDto>(),
                // EF Core map trực tiếp qua s.Tags
                Tags = s.Tags?.Select(t => new SeriesDto.TagSimpleDto
                {
                    TagId = t.Tagid,
                    TagName = t.Tagname
                }).ToList() ?? new List<SeriesDto.TagSimpleDto>()
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
                Coverimageurl = s.Coverimageurl,
                Agerating = s.Agerating,
                Createdat = s.Createdat,
                Approvedat = s.Approvedat,
                Isdeleted = s.Isdeleted,
                Genres = s.Genres?.Select(g => new SeriesDto.GenreSimpleDto
                {
                    GenreId = g.Genreid,
                    GenreName = g.Genrename
                }).ToList() ?? new List<SeriesDto.GenreSimpleDto>(),
                Tags = s.Tags?.Select(t => new SeriesDto.TagSimpleDto
                {
                    TagId = t.Tagid,
                    TagName = t.Tagname
                }).ToList() ?? new List<SeriesDto.TagSimpleDto>()
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
                    Coverimageurl = s.Coverimageurl,
                    Agerating = s.Agerating,
                    Createdat = s.Createdat,
                    Approvedat = s.Approvedat,
                    Isdeleted = s.Isdeleted,
                    Genres = s.Genres?.Select(g => new SeriesDto.GenreSimpleDto
                    {
                        GenreId = g.Genreid,
                        GenreName = g.Genrename
                    }).ToList() ?? new List<SeriesDto.GenreSimpleDto>(),
                    Tags = s.Tags?.Select(t => new SeriesDto.TagSimpleDto
                    {
                        TagId = t.Tagid,
                        TagName = t.Tagname
                    }).ToList() ?? new List<SeriesDto.TagSimpleDto>()
                }).ToList();
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
                series.Genres = seriesDto.GenreIds.Select(gid => new Genre { Genreid = gid }).ToList();
            }

            if (seriesDto.TagIds != null && seriesDto.TagIds.Any())
            {
                series.Tags = seriesDto.TagIds.Select(tid => new Tag { Tagid = tid }).ToList();
            }

            return await _seriesRepository.CreateAsync(series);
        }

        public async Task<bool> UpdateAsync(int id, SeriesDto.Update seriesDto, string proposalFileUrl, string coverImageUrl)
        {
            var existing = await _seriesRepository.GetByIdAsync(id);
            if (existing == null) return false;

            if (!string.IsNullOrEmpty(seriesDto.Title)) existing.Title = seriesDto.Title;
            if (!string.IsNullOrEmpty(seriesDto.Synopsis)) existing.Synopsis = seriesDto.Synopsis;
            if (!string.IsNullOrEmpty(seriesDto.Agerating)) existing.Agerating = seriesDto.Agerating;
            //if (!string.IsNullOrEmpty(seriesDto.Publishformat)) existing.Publishformat = seriesDto.Publishformat;
            //if (!string.IsNullOrEmpty(seriesDto.Status)) existing.Status = seriesDto.Status;
            //if (seriesDto.Isdeleted.HasValue) existing.Isdeleted = seriesDto.Isdeleted;

            if (!string.IsNullOrEmpty(proposalFileUrl)) existing.Proposalfileurl = proposalFileUrl;
            if (!string.IsNullOrEmpty(coverImageUrl)) existing.Coverimageurl = coverImageUrl;

            if (seriesDto.GenreIds != null)
            {
                existing.Genres.Clear(); 
                foreach (var gid in seriesDto.GenreIds)
                {
                    
                    existing.Genres.Add(new Genre { Genreid = gid });
                }
            }

            if (seriesDto.TagIds != null)
            {
                existing.Tags.Clear();
                foreach (var tid in seriesDto.TagIds)
                {
                    existing.Tags.Add(new Tag { Tagid = tid });
                }
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

            existing.Isdeleted = true;

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