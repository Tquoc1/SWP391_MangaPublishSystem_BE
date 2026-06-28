using Repositories.Repository;
using DTOs;
using Services.Interface;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class PageIssueService : IPageIssueService
    {
        private static readonly Dictionary<string, List<string>> _validTransitions = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Reported", new List<string> { "InProgress", "Cancelled" } },
            { "InProgress", new List<string> { "Resolved", "Cancelled" } },
            { "Resolved", new List<string> { "Completed", "Cancelled" } }
        };

        private readonly PageIssueRepository _pageIssueRepository;
        private readonly INotificationService _notificationService;

        public PageIssueService(PageIssueRepository pageIssueRepository, INotificationService notificationService)
        {
            _pageIssueRepository = pageIssueRepository;
            _notificationService = notificationService;
        }

        public async Task<List<PageIssueDto>> GetAllAsync(int? chapterId, int? pageId, string? status, string? workCategory)
        {
            var issues = await _pageIssueRepository.GetIssuesAsync(chapterId, pageId, status, workCategory);
            return issues.Select(MapToDto).ToList();
        }

        public async Task<PageIssueDto> GetByIdAsync(int id)
        {
            var issue = await _pageIssueRepository.GetIssueByIdAsync(id);
            return issue != null ? MapToDto(issue) : null;
        }

        public async Task<int> CreateAsync(PageIssueDto.Create dto)
        {
            var issue = new PageIssue
            {
                Pageid = dto.Pageid,
                CreatedById = dto.CreatedById,
                AssignedToId = dto.AssignedToId,
                IssueType = dto.IssueType,
                WorkCategory = dto.WorkCategory,
                BoxX = dto.BoxX,
                BoxY = dto.BoxY,
                BoxWidth = dto.BoxWidth,
                BoxHeight = dto.BoxHeight,
                Description = dto.Description,
                Status = "Reported",
                Deadline = dto.Deadline,
                Createdat = DateTime.UtcNow,
                Isdeleted = false
            };

            await _pageIssueRepository.CreateAsync(issue);
            
            if (dto.AssignedToId.HasValue)
            {
                await _notificationService.CreateNotificationAsync(
                    dto.AssignedToId.Value,
                    "Công việc mới được giao",
                    $"Bạn vừa được giao một công việc mới ở trang {dto.Pageid}."
                );
            }
            
            return issue.Issueid;
        }

        public async Task UpdateStatusAsync(int id, PageIssueDto.UpdateStatus dto)
        {
            var existing = await _pageIssueRepository.GetByIdAsync(id);

            if (existing == null || existing.Isdeleted == true)
                throw new KeyNotFoundException("Không tìm thấy sự cố (Issue).");

            if (string.Equals(existing.Status, dto.Status, StringComparison.OrdinalIgnoreCase))
                return;

            if (!_validTransitions.ContainsKey(existing.Status) || 
                !_validTransitions[existing.Status].Contains(dto.Status, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Không thể chuyển trạng thái từ '{existing.Status}' sang '{dto.Status}'. Luồng không hợp lệ!");
            }

            existing.Status = dto.Status;

            await _pageIssueRepository.UpdateAsync(existing);
            
            if (dto.Status == "Resolved" && existing.CreatedById > 0)
            {
                await _notificationService.CreateNotificationAsync(
                    existing.CreatedById,
                    "Công việc đã hoàn thành",
                    $"Assistant đã hoàn thành công việc ở trang {existing.Pageid}."
                );
            }

            return;
        }
        public async Task UpdateAsync(int id, PageIssueDto.Update dto)
        {
            var existing = await _pageIssueRepository.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Không tìm thấy sự cố cần cập nhật.");

            existing.AssignedToId = dto.AssignedToId;
            existing.Description = dto.Description;
            existing.BoxX = dto.BoxX;
            existing.BoxY = dto.BoxY;
            existing.BoxWidth = dto.BoxWidth;
            existing.BoxHeight = dto.BoxHeight;
            existing.Deadline = dto.Deadline;
            existing.Completedat = dto.Completedat;

            await _pageIssueRepository.UpdateAsync(existing);
        }

        public async Task UpdateStatusAsync(int id, string status)
        {
            var existing = await _pageIssueRepository.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Không tìm thấy sự cố.");

            if (string.Equals(existing.Status, status, StringComparison.OrdinalIgnoreCase))
                return;

            if (!_validTransitions.ContainsKey(existing.Status) || 
                !_validTransitions[existing.Status].Contains(status, StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Không thể chuyển trạng thái từ '{existing.Status}' sang '{status}'. Luồng không hợp lệ!");
            }

            existing.Status = status;
            await _pageIssueRepository.UpdateAsync(existing);
            
            if (status == "Resolved" && existing.CreatedById > 0)
            {
                await _notificationService.CreateNotificationAsync(
                    existing.CreatedById,
                    "Công việc đã hoàn thành",
                    $"Assistant đã hoàn thành công việc ở trang {existing.Pageid}."
                );
            }
            
        }

        public async Task SoftDeleteAsync(int id)
        {
            var existing = await _pageIssueRepository.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException("Không tìm thấy sự cố để xóa.");

            existing.Isdeleted = true;
            await _pageIssueRepository.UpdateAsync(existing);
        }

        public async Task RemoveAsync(int id)
        {
            var existing = await _pageIssueRepository.GetByIdAsync(id);

            if (existing == null || existing.Isdeleted == true)
                throw new KeyNotFoundException("Không tìm thấy sự cố.");

            existing.Isdeleted = true;

            await _pageIssueRepository.UpdateAsync(existing);
        }

        private PageIssueDto MapToDto(PageIssue issue)
        {
            return new PageIssueDto
            {
                Issueid = issue.Issueid,
                Pageid = issue.Pageid,
                CreatedById = issue.CreatedById,
                AssignedToId = issue.AssignedToId,
                IssueType = issue.IssueType,
                WorkCategory = issue.WorkCategory,
                BoxX = issue.BoxX,
                BoxY = issue.BoxY,
                BoxWidth = issue.BoxWidth,
                BoxHeight = issue.BoxHeight,
                Description = issue.Description,
                Status = issue.Status,
                Deadline = issue.Deadline,
                Createdat = issue.Createdat,
                Completedat = issue.Completedat,
                Isdeleted = issue.Isdeleted
            };
        }
    }
}
