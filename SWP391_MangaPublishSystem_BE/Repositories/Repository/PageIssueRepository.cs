using Entities.Models;
using Membership.Repositories.QuocDT.Base;
using Microsoft.EntityFrameworkCore;
using DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class PageIssueRepository : GenericRepository<PageIssue>
    {
        public PageIssueRepository() { }
        public PageIssueRepository(MangaPublishDBContext context) : base(context) => _context = context;

        // Get all active issues, optionally filtered by chapter
        public async Task<List<PageIssueDto>> GetAllActiveAsync(int? chapterId = null)
        {
            var query = _context.PageIssues.Where(i => i.Isdeleted == false || i.Isdeleted == null);

            if (chapterId.HasValue)
            {
                // Get all page IDs in the chapter and filter issues by those pages
                var pageIds = await _context.Pages
                    .Where(p => p.Chapterid == chapterId.Value)
                    .Select(p => p.Pageid)
                    .ToListAsync();

                query = query.Where(i => pageIds.Contains(i.Pageid));
            }

            var issues = await query.ToListAsync();
            return issues.Select(MapToDto).ToList();
        }

        // Get active issue by id
        public async Task<PageIssueDto> GetByIdActiveAsync(int id)
        {
            var issue = await _context.PageIssues
                .FirstOrDefaultAsync(i => i.Issueid == id && (i.Isdeleted == false || i.Isdeleted == null));

            return issue != null ? MapToDto(issue) : null;
        }

        // Get issues by page id
        public async Task<List<PageIssueDto>> GetByPageIdAsync(int pageId)
        {
            var issues = await _context.PageIssues
                .Where(i => i.Pageid == pageId && (i.Isdeleted == false || i.Isdeleted == null))
                .ToListAsync();

            return issues.Select(MapToDto).ToList();
        }

        // Create issue with default values
        public async Task<int> CreateWithDefaultsAsync(PageIssueDto.Create pageDto)
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

            _context.PageIssues.Add(issue);
            await _context.SaveChangesAsync();
            return issue.Issueid;
        }

        // Update issue
        public async Task<int> UpdateIssueAsync(int id, PageIssueDto.Update pageDto)
        {
            var existing = await _context.PageIssues.FindAsync(id);
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

            _context.ChangeTracker.Clear();
            var tracker = _context.Attach(existing);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        // Soft delete issue
        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.PageIssues.FindAsync(id);
            if (existing == null) return false;

            existing.Isdeleted = true;
            _context.ChangeTracker.Clear();
            var tracker = _context.Attach(existing);
            tracker.State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        // Map entity to DTO
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
