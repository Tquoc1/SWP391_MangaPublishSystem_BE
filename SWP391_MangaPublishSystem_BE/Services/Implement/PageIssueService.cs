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
        private readonly PageIssueRepository _pageIssueRepository;

        public PageIssueService(PageIssueRepository pageIssueRepository)
        {
            _pageIssueRepository = pageIssueRepository;
        }

        public async Task<List<PageIssueDto>> GetAllAsync(int? chapterId)
        {
            var issues = await _pageIssueRepository.GetIssuesAsync(chapterId);
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
                Status = "Pending",
                Deadline = dto.Deadline,
                Createdat = DateTime.UtcNow,
                Isdeleted = false
            };

            await _pageIssueRepository.CreateAsync(issue);
            return issue.Issueid;
        }

        public async Task<int> UpdateAsync(int id, PageIssueDto.Update dto)
        {
            var existing = await _pageIssueRepository.GetByIdAsync(id);
            if (existing == null) return 0;

            existing.AssignedToId = dto.AssignedToId;
            existing.Status = dto.Status;
            existing.Description = dto.Description;
            existing.BoxX = dto.BoxX;
            existing.BoxY = dto.BoxY;
            existing.BoxWidth = dto.BoxWidth;
            existing.BoxHeight = dto.BoxHeight;
            existing.Deadline = dto.Deadline;
            existing.Completedat = dto.Completedat;

            if (dto.Isdeleted.HasValue)
            {
                existing.Isdeleted = dto.Isdeleted.Value;
            }

            await _pageIssueRepository.UpdateAsync(existing);
            return 1;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var existing = await _pageIssueRepository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.Isdeleted = true;
            await _pageIssueRepository.UpdateAsync(existing);
            return true;
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
