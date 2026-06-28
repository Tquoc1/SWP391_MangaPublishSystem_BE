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
        private static readonly Dictionary<string, List<string>> _validTransitions = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Draft", new List<string> { "EditorReview", "Cancelled" } },
            { "EditorReview", new List<string> { "EBReview", "Cancelled" } },
            { "EBReview", new List<string> { "Publishing", "Cancelled" } },
            { "Publishing", new List<string> { "Completed", "Cancelled" } }
        };

        private readonly SeriesRepository _seriesRepository;
        private readonly INotificationService _notificationService;
        private readonly UserRepository _userRepository;

        public SeriesService(SeriesRepository seriesRepository, INotificationService notificationService, UserRepository userRepository)
        {
            _seriesRepository = seriesRepository;
            _notificationService = notificationService;
            _userRepository = userRepository;
        }

        public async Task<List<SeriesDto>> GetAllAsync(int? mangakaId, string? status)
        {
            var seriesList = await _seriesRepository.GetAllWithDetailsAsync(mangakaId, status);
            return seriesList.Select(MapToDto).ToList();
        }

        public async Task<SeriesDto> GetByIdAsync(int id)
        {
            var series = await _seriesRepository.GetByIdWithDetailsAsync(id);
            return series != null ? MapToDto(series) : null;
        }

        public async Task<List<SeriesDto>> GetByMangakaIdAsync(int mangakaId)
        {
            return await GetAllAsync(mangakaId, null);
        }

        public async Task<int> CreateAsync(SeriesDto.Create seriesDto, string proposalFileUrl, string coverImageUrl)
        {
            var series = new Series
            {
                Title = seriesDto.Title,
                Synopsis = seriesDto.Synopsis,
                Mangakaid = seriesDto.Mangakaid,
                Agerating = seriesDto.Agerating ?? "G",
                Tantoueditorid = null,
                Publishformat = "Pending",
                Proposalfileurl = proposalFileUrl,
                Coverimageurl = coverImageUrl,
                Status = "Draft",
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

            var admins = await _userRepository.GetAdminsAsync();
            foreach (var admin in admins)
            {
                await _notificationService.CreateNotificationAsync(
                    admin.Userid,
                    "Tác phẩm mới chờ duyệt",
                    $"Tác phẩm '{series.Title}' vừa được gửi lên hệ thống và đang chờ phê duyệt.",
                    series.Seriesid
                );
            }

            return series.Seriesid;
        }

        public async Task UpdateAsync(int id, SeriesDto.Update seriesDto, string proposalFileUrl, string coverImageUrl)
        {
            var existing = await _seriesRepository.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Không tìm thấy tác phẩm cần cập nhật.");

            existing.Title = seriesDto.Title;
            existing.Synopsis = seriesDto.Synopsis;
            existing.Agerating = seriesDto.Agerating;
            existing.Proposalfileurl = proposalFileUrl;
            existing.Coverimageurl = coverImageUrl;

            await _seriesRepository.UpdateWithDetailsAsync(existing, seriesDto.GenreIds, seriesDto.TagIds);
        }

        public async Task UpdateStatusAsync(int id, SeriesDto.UpdateStatus seriesDto)
        {
            var existing = await _seriesRepository.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Không tìm thấy tác phẩm.");

            if (string.Equals(existing.Status, seriesDto.Status, StringComparison.OrdinalIgnoreCase))
                return;

            if (!_validTransitions.ContainsKey(existing.Status) || 
                !_validTransitions[existing.Status].Contains(seriesDto.Status, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Không thể chuyển trạng thái từ '{existing.Status}' sang '{seriesDto.Status}'. Luồng không hợp lệ!");
            }

            existing.Status = seriesDto.Status;
            if (seriesDto.Status.Equals("Publishing", StringComparison.OrdinalIgnoreCase))
            {
                existing.Approvedat = DateTime.UtcNow;
            }

            await _seriesRepository.UpdateAsync(existing);
            
            if (existing.Mangakaid > 0)
            {
                string statusVn = existing.Status == "Publishing" ? "Đã duyệt phát hành" : (existing.Status == "Cancelled" ? "Đã hủy" : existing.Status);
                await _notificationService.CreateNotificationAsync(
                    existing.Mangakaid,
                    "Cập nhật trạng thái Tác phẩm",
                    $"Tác phẩm '{existing.Title}' của bạn đã được chuyển sang trạng thái: {statusVn}.",
                    existing.Seriesid
                );
            }
        }

        public async Task UpdatePublishFormatAsync(int id, SeriesDto.UpdatePublishFormat seriesDto)
        {
            var existing = await _seriesRepository.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Không tìm thấy tác phẩm.");

            existing.Publishformat = seriesDto.Publishformat;
            await _seriesRepository.UpdateAsync(existing);
        }

        public async Task UploadCoverAsync(int id, string coverImageUrl)
        {
            var existing = await _seriesRepository.GetByIdWithDetailsAsync(id);

            if (existing == null || existing.Isdeleted == true)
                throw new KeyNotFoundException("Không tìm thấy tác phẩm.");

            existing.Coverimageurl = coverImageUrl;

            await _seriesRepository.UpdateAsync(existing);
        }

        public async Task UploadProposalAsync(int id, string proposalFileUrl)
        {
            var existing = await _seriesRepository.GetByIdWithDetailsAsync(id);

            if (existing == null || existing.Isdeleted == true)
                throw new KeyNotFoundException("Không tìm thấy tác phẩm.");

            existing.Proposalfileurl = proposalFileUrl;

            await _seriesRepository.UpdateAsync(existing);
        }

        public async Task SoftDeleteAsync(int id)
        {
            var existing = await _seriesRepository.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Không tìm thấy tác phẩm.");

            existing.Isdeleted = true;
            await _seriesRepository.UpdateAsync(existing);
        }

        public async Task RemoveAsync(int id)
        {
            await SoftDeleteAsync(id);
        }

        public async Task UpdateTantouEditorAsync(int id, int tantouEditorId)
        {
            var existing = await _seriesRepository.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Không tìm thấy tác phẩm.");

            var editor = await _userRepository.GetUserById(tantouEditorId);
            if (editor == null || editor.Roleid != 3 || editor.Isdeleted == true || editor.Status != "Active")
            {
                throw new ArgumentException("Mã biên tập viên không hợp lệ hoặc biên tập viên không hoạt động.");
            }

            existing.Tantoueditorid = tantouEditorId;
            await _seriesRepository.UpdateAsync(existing);
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
