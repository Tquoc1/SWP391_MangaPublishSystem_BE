using Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IPageService
    {
        Task<List<PageDto>> GetAllAsync(int? chapterId);
        Task<PageDto> GetByIdAsync(int id);
        Task<int> CreateAsync(PageDto.Create pageDto, string pageImageUrl);
        Task<int> UpdateAsync(int id,PageDto.Update pageDto, string pageImageUrl);
        Task<bool> RemoveAsync(int id);
    }
}
