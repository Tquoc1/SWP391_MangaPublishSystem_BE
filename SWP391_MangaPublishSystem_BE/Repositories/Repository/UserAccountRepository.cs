using Entities.Models;
using Membership.Repositories.QuocDT.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Repository
{
    public class UserAccountRepository : GenericRepository<User>
    {
        public UserAccountRepository() { }
        public UserAccountRepository(MangaPublishDBContext context) : base(context) =>  _context = context ;
        public async Task<User> GetUserAccount(string userName, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Username == userName && x.Passwordhash == password);
        }
    }
}
