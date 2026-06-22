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
        Task UpdateAsync(int id, PageIssueDto.Update pageDto);
        Task UpdateStatusAsync(int id, string status);
        Task SoftDeleteAsync(int id);
        Task RemoveAsync(int id);
        Task UpdateStatusAsync(int id, PageIssueDto.UpdateStatus dto);
    }
}
