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
        private readonly PageRepository _pageRepository;
        private readonly IPageLayerService _pageLayerService;
        private readonly IFileStorageService _fileStorage;

        public PageService(PageRepository pageRepository, IPageLayerService pageLayerService, IFileStorageService fileStorage)
        {
            _pageRepository = pageRepository;
            _pageLayerService = pageLayerService;
            _fileStorage = fileStorage;
        }

        public async Task<List<PageDto>> GetAllAsync(int? chapterId)
        {
            var pages = await _pageRepository.GetPagesAsync(chapterId);
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
                Status = "Draft",
                Isdeleted = false
            };

            await _pageRepository.CreateAsync(page);
            return page.Pageid;
        }

        public async Task<int> UpdateAsync(int id, PageDto.Update pageDto)
        {
            var existing = await _pageRepository.GetByIdAsync(id);
            if (existing == null) return 0;

            existing.Pagenumber = pageDto.Pagenumber;

            await _pageRepository.UpdateAsync(existing);
            return 1;
        }

        public async Task<string> CompositeAndSaveImageAsync(int id)
        {
            var page = await _pageRepository.GetByIdAsync(id);
            if (page == null) return null;

            var layers = await _pageLayerService.GetAllAsync(id);
            var visibleLayers = layers.Where(l => l.Isvisible == true && !l.Isdeleted).OrderBy(l => l.Zindex).ToList();

            if (!visibleLayers.Any()) return null;

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

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            var existing = await _pageRepository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.Status = status;
            await _pageRepository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var existing = await _pageRepository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.Isdeleted = true;
            await _pageRepository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var existing = await _pageRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _pageRepository.RemoveAsync(existing);
            return true;
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
