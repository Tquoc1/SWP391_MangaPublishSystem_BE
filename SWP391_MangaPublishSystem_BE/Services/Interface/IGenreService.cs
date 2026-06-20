using DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IGenreService
    {
        Task<List<GenreDto>> GetAllAsync();
        Task<GenreDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(GenreDto.Create createDto);
        Task<bool> UpdateAsync(int id, GenreDto.Update updateDto);
        Task<bool> RemoveAsync(int id);
    }
}
