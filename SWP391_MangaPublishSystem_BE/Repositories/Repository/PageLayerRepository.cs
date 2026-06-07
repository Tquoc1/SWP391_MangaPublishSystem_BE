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
    public class PageLayerRepository : GenericRepository<Pagelayer>
    {
        public PageLayerRepository() { }
        public PageLayerRepository(MangaPublishDBContext context) : base(context) => _context = context;

    }
}
