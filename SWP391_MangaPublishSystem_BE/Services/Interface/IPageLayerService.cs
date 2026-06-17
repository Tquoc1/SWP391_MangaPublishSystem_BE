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
        Task<List<PageLayerDto>> GetAllAsync(int? chapterId);
        Task<PageLayerDto> GetByIdAsync(int id);
        Task<int> CreateAsync(PageLayerDto.Create pageDto, string fileUrl);
        Task<int> UpdateAsync(int id, PageLayerDto.Update pageDto, string fileUrl);
        Task<bool> RemoveAsync(int id);
    }
}
