using Entities.Models;
using Membership.Repositories.QuocDT.Base;
using Microsoft.EntityFrameworkCore;
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
    }
}
