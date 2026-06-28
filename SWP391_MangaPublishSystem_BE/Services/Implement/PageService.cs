using Repositories.Repository;
using DTOs;
using Services.Interface;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;
using System.Net.Http;

namespace Services.Implement
{
    public class PageService : IPageService
    {
        private static readonly Dictionary<string, List<string>> _validTransitions = new(StringComparer.OrdinalIgnoreCase)
        {
            { "InWork", new List<string> { "Reviewing" } },
            { "Reviewing", new List<string> { "Approved", "InWork" } }
        };

        private readonly PageRepository _pageRepository;
        private readonly IPageLayerService _pageLayerService;
        private readonly IFileStorageService _fileStorage;

        public PageService(PageRepository pageRepository, IPageLayerService pageLayerService, IFileStorageService fileStorage)
        {
            _pageRepository = pageRepository;
            _pageLayerService = pageLayerService;
            _fileStorage = fileStorage;
        }

        public async Task<List<PageDto>> GetAllAsync(int? chapterId, string? status)
        {
            var pages = await _pageRepository.GetPagesAsync(chapterId, status);
            return pages.Select(MapToDto).ToList();
        }

        public async Task<PageDto> GetByIdAsync(int id)
        {
            var page = await _pageRepository.GetPageByIdAsync(id);
            return page != null ? MapToDto(page) : null;
        }

        public async Task<int> CreateAsync(PageDto.Create pageDto)
        {
            var page = new Page
            {
                Chapterid = pageDto.Chapterid,
                Pagenumber = pageDto.Pagenumber,
                Pageimageurl = string.Empty,
                Status = "InWork",
                Isdeleted = false
            };

            await _pageRepository.CreateAsync(page);
            return page.Pageid;
        }

        public async Task UpdateAsync(int id, PageDto.Update pageDto)
        {
            var existing = await _pageRepository.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Không tìm thấy trang truyện cần cập nhật.");

            existing.Pagenumber = pageDto.Pagenumber;

            await _pageRepository.UpdateAsync(existing);
        }

        public async Task<string> CompositeAndSaveImageAsync(int id)
        {
            var page = await _pageRepository.GetByIdAsync(id);
            if (page == null) throw new KeyNotFoundException("Không tìm thấy trang truyện.");

            var layers = await _pageLayerService.GetAllAsync(id);
            var visibleLayers = layers.Where(l => l.Isvisible == true && !l.Isdeleted).OrderBy(l => l.Zindex).ToList();

            if (!visibleLayers.Any()) throw new InvalidOperationException("Không có layer hợp lệ để ghép ảnh.");

            using var httpClient = new HttpClient();
            Image<Rgba32> compositeImage = null;

            foreach (var layer in visibleLayers)
            {
                if (string.IsNullOrEmpty(layer.Fileurl)) continue;

                var imageBytes = await httpClient.GetByteArrayAsync(layer.Fileurl);
                using var currentImage = Image.Load<Rgba32>(imageBytes);
                
                // Apply opacity
                if (layer.Opacity < 1.0m)
                {
                    currentImage.Mutate(x => x.Opacity((float)layer.Opacity));
                }

                if (compositeImage == null)
                {
                    compositeImage = currentImage.Clone();
                }
                else
                {
                    // Draw current image on top of composite image
                    compositeImage.Mutate(x => x.DrawImage(currentImage, new Point(0, 0), 1f));
                }
            }

            if (compositeImage == null) return null;

            using var memoryStream = new MemoryStream();
            await compositeImage.SaveAsync(memoryStream, new PngEncoder());
            memoryStream.Position = 0;

            string fileName = $"page_{id}_composite_{DateTime.UtcNow.Ticks}.png";
            string newImageUrl = await _fileStorage.UploadAsync(memoryStream, fileName, "image/png", "manga-pages");

            page.Pageimageurl = newImageUrl;
            await _pageRepository.UpdateAsync(page);

            return newImageUrl;
        }

        public async Task UpdateStatusAsync(int id, string status)
        {
            var existing = await _pageRepository.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Không tìm thấy trang truyện để cập nhật trạng thái.");

            if (string.Equals(existing.Status, status, StringComparison.OrdinalIgnoreCase))
                return;

            if (!_validTransitions.ContainsKey(existing.Status) || 
                !_validTransitions[existing.Status].Contains(status, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Không thể chuyển trạng thái từ '{existing.Status}' sang '{status}'. Luồng không hợp lệ!");
            }

            existing.Status = status;
            await _pageRepository.UpdateAsync(existing);
        }

        public async Task SoftDeleteAsync(int id)
        {
            var existing = await _pageRepository.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Không tìm thấy trang truyện để xóa tạm.");

            existing.Isdeleted = true;
            await _pageRepository.UpdateAsync(existing);
        }

        public async Task RemoveAsync(int id)
        {
            var existing = await _pageRepository.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Không tìm thấy trang truyện.");

            await _pageRepository.RemoveAsync(existing);
        }

        private PageDto MapToDto(Page page)
        {
            return new PageDto
            {
                Pageid = page.Pageid,
                Chapterid = page.Chapterid,
                Pagenumber = page.Pagenumber,
                Pageimageurl = page.Pageimageurl,
                Status = page.Status,
                Isdeleted = page.Isdeleted
            };
        }
    }
}
