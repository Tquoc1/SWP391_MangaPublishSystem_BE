using DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ITagService
    {
        Task<List<TagDto>> GetAllAsync();
        Task<TagDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(TagDto.Create createDto);
        Task UpdateAsync(int id, TagDto.Update updateDto);
        Task RemoveAsync(int id);
    }
}
