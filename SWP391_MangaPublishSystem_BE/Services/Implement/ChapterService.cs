using Repositories.Repository;
using DTOs;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class ChapterService : IChapterService
    {
        private readonly ChapterRepository _chapterRepository;

        public ChapterService(ChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }

        public async Task<List<ChapterDto>> GetAllAsync(int? seriesId = null)
        {
            return await _chapterRepository.GetAllActiveAsync(seriesId);
        }

        public async Task<ChapterDto> GetByIdAsync(int id)
        {
            return await _chapterRepository.GetByIdActiveAsync(id);
        }

        public async Task<int> CreateAsync(ChapterDto.Create chapterDto)
        {
            return await _chapterRepository.CreateWithDefaultsAsync(
                chapterDto.Seriesid,
                chapterDto.Chapternumber,
                chapterDto.Title,
                chapterDto.Deadline
            );
        }

        public async Task<int> UpdateAsync(ChapterDto.Update chapterDto)
        {
            return await _chapterRepository.UpdateChapterAsync(
                chapterDto.Chapterid,
                chapterDto.Chapternumber,
                chapterDto.Title,
                chapterDto.Deadline
                //chapterDto.Status,
                //chapterDto.Isdeleted
            );
        }

        public async Task<bool> RemoveAsync(int id)
        {
            return await _chapterRepository.DeleteAsync(id);
        }
    }
}
