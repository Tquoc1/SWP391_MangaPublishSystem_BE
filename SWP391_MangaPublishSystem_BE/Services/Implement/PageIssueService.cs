using Entities.Models;
using Repositories.Repository;
using Services.DTO;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implement
{
    public class PageIssueService : IPageIssueService
    {
        private readonly PageIssueRepository _pageIssueRepository;
        private readonly PageRepository _pageRepository; 

        public PageIssueService(PageIssueRepository pageIssueRepository, PageRepository pageRepository)
        {
            _pageIssueRepository = pageIssueRepository;
            _pageRepository = pageRepository;
        }

        public async Task<List<PageIssueDto>> GetAllAsync(int? chapterId)
        {
            var issues = await _pageIssueRepository.GetAllAsync();

            var query = issues.Where(i => i.Isdeleted == false || i.Isdeleted == null);
            if (chapterId.HasValue)
            {
                var pagesInChapter = await _pageRepository.GetAllAsync();

                var pageIds = pagesInChapter
                    .Where(p => p.Chapterid == chapterId.Value)
                    .Select(p => p.Pageid)
                    .ToList();
                query = query.Where(i => pageIds.Contains(i.Pageid));
            }

            return query.Select(i => new PageIssueDto
            {
                Issueid = i.Issueid,
                Pageid = i.Pageid,
                CreatedById = i.CreatedById,
                AssignedToId = i.AssignedToId,
                IssueType = i.IssueType,
                WorkCategory = i.WorkCategory,
                BoxX = i.BoxX,
                BoxY = i.BoxY,
                BoxWidth = i.BoxWidth,
                BoxHeight = i.BoxHeight,
                Description = i.Description,
                Status = i.Status,
                Deadline = i.Deadline,
                Createdat = i.Createdat,
                Completedat = i.Completedat,
                Isdeleted = i.Isdeleted
            }).ToList();
        }

        public async Task<PageIssueDto> GetByIdAsync(int id)
        {
            var i = await _pageIssueRepository.GetByIdAsync(id);
            if (i == null || i.Isdeleted == true) return null;

            return new PageIssueDto
            {
                Issueid = i.Issueid,
                Pageid = i.Pageid,
                CreatedById = i.CreatedById,
                AssignedToId = i.AssignedToId,
                IssueType = i.IssueType,
                WorkCategory = i.WorkCategory,
                BoxX = i.BoxX,
                BoxY = i.BoxY,
                BoxWidth = i.BoxWidth,
                BoxHeight = i.BoxHeight,
                Description = i.Description,
                Status = i.Status,
                Deadline = i.Deadline,
                Createdat = i.Createdat,
                Completedat = i.Completedat,
                Isdeleted = i.Isdeleted
            };
        }

        public Task<int> CreateAsync(PageIssueDto.Create pageDto)
        {
            var issue = new PageIssue
            {
                Pageid = pageDto.Pageid,
                CreatedById = pageDto.CreatedById,
                AssignedToId = pageDto.AssignedToId,
                IssueType = pageDto.IssueType,
                WorkCategory = pageDto.WorkCategory,
                BoxX = pageDto.BoxX,
                BoxY = pageDto.BoxY,
                BoxWidth = pageDto.BoxWidth,
                BoxHeight = pageDto.BoxHeight,
                Description = pageDto.Description,
                Status = "Pending", 
                Deadline = pageDto.Deadline,
                Createdat = DateTime.UtcNow,
                Isdeleted = false
            };
            return _pageIssueRepository.CreateAsync(issue);
        }

        public async Task<int> UpdateAsync(int id, PageIssueDto.Update pageDto)
        {
            var existing = await _pageIssueRepository.GetByIdAsync(id);
            if (existing == null) return 0;

            existing.AssignedToId = pageDto.AssignedToId;
            existing.Status = pageDto.Status;
            existing.Description = pageDto.Description;
            existing.BoxX = pageDto.BoxX;
            existing.BoxY = pageDto.BoxY;
            existing.BoxWidth = pageDto.BoxWidth;
            existing.BoxHeight = pageDto.BoxHeight;
            existing.Deadline = pageDto.Deadline;
            existing.Completedat = pageDto.Completedat;

            if (pageDto.Isdeleted.HasValue)
            {
                existing.Isdeleted = pageDto.Isdeleted.Value;
            }

            return await _pageIssueRepository.UpdateAsync(existing);
        }

        public async Task<bool> RemoveAsync(int id)
        {
            var existing = await _pageIssueRepository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.Isdeleted = true; 
            var result = await _pageIssueRepository.UpdateAsync(existing);
            return result > 0;
        }
    }
}
