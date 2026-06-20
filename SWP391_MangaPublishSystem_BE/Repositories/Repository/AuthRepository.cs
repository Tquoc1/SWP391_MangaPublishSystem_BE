using Entities.Models;
using Membership.Repositories.QuocDT.Base;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class AuthRepository : GenericRepository<User>
    {
        public AuthRepository() { }
        public AuthRepository(MangaPublishDBContext context) : base(context) => _context = context;

        public async Task<User> GetUserByUsername(string userName)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Username == userName);
        }

        public async Task<UserToken> GetUserToken(string token)
        {
            return await _context.UserTokens.FirstOrDefaultAsync(x => x.Token == token);
        }

        public async Task CreateUserToken(UserToken token)
        {
            _context.UserTokens.Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserToken(UserToken token)
        {
            _context.UserTokens.Update(token);
            await _context.SaveChangesAsync();
        }
    }
}