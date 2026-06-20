using Entities.Models;
using Repositories.Repository;
using DTOs;
using Services.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class GenreService : IGenreService
    {
        private readonly GenreRepository _genreRepository;

        public GenreService(GenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public async Task<List<GenreDto>> GetAllAsync()
        {
            var genres = await _genreRepository.GetAllAsync();
            return genres.Select(g => new GenreDto
            {
                Genreid = g.Genreid,
                Genrename = g.Genrename,
                Description = g.Description
            }).ToList();
        }

        public async Task<GenreDto?> GetByIdAsync(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null) return null;

            return new GenreDto
            {
                Genreid = genre.Genreid,
                Genrename = genre.Genrename,
                Description = genre.Description
            };
        }

        public async Task<int> CreateAsync(GenreDto.Create createDto)
        {
            var genre = new Genre
            {
                Genrename = createDto.Genrename,
                Description = createDto.Description
            };

            await _genreRepository.CreateAsync(genre);
            return genre.Genreid;
        }

        public async Task<bool> UpdateAsync(int id, GenreDto.Update updateDto)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null) return false;

            genre.Genrename = updateDto.Genrename;
            genre.Description = updateDto.Description;

            return await _genreRepository.UpdateAsync(genre) > 0;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null) return false;

            return await _genreRepository.RemoveAsync(genre);
        }
    }
}
