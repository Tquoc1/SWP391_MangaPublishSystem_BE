using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IPageLayerService
    {
        Task<List<PageLayerDto>> GetAllAsync(int? pageId);
        Task<PageLayerDto> GetByIdAsync(int id);
        Task<int> CreateAsync(PageLayerDto.Create dto, string fileUrl);
        Task<int> UpdateAsync(int id, PageLayerDto.Update dto, string fileUrl);
        Task<bool> ToggleVisibilityAsync(int id);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> RemoveAsync(int id);
    }
}
