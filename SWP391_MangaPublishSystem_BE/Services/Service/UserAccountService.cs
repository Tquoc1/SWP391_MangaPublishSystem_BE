using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class UserAccountService : Interface.IUserAccountService
    {
        private Repositories.Repository.UserAccountRepository _userAccountRepository;
        public UserAccountService() { }
        public UserAccountService(Repositories.Repository.UserAccountRepository userAccountRepository) => _userAccountRepository = userAccountRepository;
        public async Task<Entities.Models.User> GetUserAccount(string userName, string password)
        {
            try
            {
                return await _userAccountRepository.GetUserAccount(userName, password);
            } catch (Exception ex)
            {
                throw;
            }
        }
    }
}
