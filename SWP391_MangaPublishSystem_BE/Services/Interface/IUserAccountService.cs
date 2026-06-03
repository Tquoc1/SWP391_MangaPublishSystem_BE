using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IUserAccountService
    {
        Task<Entities.Models.User> GetUserAccount(string userName, string password);
    }
}
