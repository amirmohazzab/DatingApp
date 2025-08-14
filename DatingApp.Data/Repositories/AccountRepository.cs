using DatingApp.Data.Context;
using DatingApp.Domain.Entities.User;
using DatingApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Data.Repositories
{
    public class AccountRepository (DatingAppDbContext dbContext) : IAccountRepository
    {
        public async Task addUser(User user)
        {
            await dbContext.Users.AddAsync(user);
        }

        public async Task<User> GetUserByUserName(string userName)
        {
            return await dbContext.Users
                .SingleOrDefaultAsync(u => u.UserName.ToLower() == userName.ToLower());
        }

        public async Task<User> GetUserByUserNameWithPhoto(string userName)
        {
            return await dbContext.Users
                .Include(u => u.Photos)
                .SingleOrDefaultAsync(u => u.UserName.ToLower() == userName.ToLower());
        }

        public async Task<bool> IsExistUserName(string userName)
        {
            return await dbContext.Users.AnyAsync(u => u.UserName.ToLower() == userName.ToLower());
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await dbContext.SaveChangesAsync() > 0;
        }
    }
}
