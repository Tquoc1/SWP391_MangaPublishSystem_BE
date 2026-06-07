using Entities.Models;
using Membership.Repositories.QuocDT.Base;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Repository
{
    public class AuthRepository : GenericRepository<User>
    {
        public AuthRepository() { }
        public AuthRepository(MangaPublishDBContext context) : base(context) => _context = context;

        public async Task<User> GetUserAccount(string userName, string passwordHash)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Username == userName && x.Passwordhash == passwordHash);
        }

        public async Task<User> GetUserByUsername(string userName)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Username == userName);
        }

        public async Task<int> CreateUserToken(UserToken token)
        {
            _context.UserTokens.Add(token);
            return await _context.SaveChangesAsync();
        }

        public async Task<UserToken> GetUserToken(string token)
        {
            return await _context.UserTokens.FirstOrDefaultAsync(x => x.Token == token);
        }

        public async Task<int> RevokeUserToken(UserToken token)
        {
            token.Isrevoked = true;
            _context.UserTokens.Update(token);
            return await _context.SaveChangesAsync();
        }
    }
}