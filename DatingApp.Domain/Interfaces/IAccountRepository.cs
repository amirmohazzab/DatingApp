using DatingApp.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Domain.Interfaces
{
    public interface IAccountRepository
    {
        Task<bool> IsExistUserName(string userName);

        Task addUser(User user);

        Task<User> GetUserByUserName(string userName);

        Task<User> GetUserByUserNameWithPhoto(string userName);

        Task<bool> SaveChangesAsync();
    }
}
