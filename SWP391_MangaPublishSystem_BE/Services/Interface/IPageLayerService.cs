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
        Task UpdateAsync(int id, PageLayerDto.Update dto, string fileUrl);
        Task ToggleVisibilityAsync(int id);
        Task SoftDeleteAsync(int id);
        Task RemoveAsync(int id);
    }
}
