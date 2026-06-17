using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IPageIssueService
    {
        Task<List<PageIssueDto>> GetAllAsync(int? chapterId);
        Task<PageIssueDto> GetByIdAsync(int id);
        Task<int> CreateAsync(PageIssueDto.Create pageDto);
        Task<int> UpdateAsync(int id, PageIssueDto.Update pageDto);
        Task<bool> RemoveAsync(int id);
    }
}
