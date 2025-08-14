using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.Data.Context;
using DatingApp.Domain.DTOs;
using DatingApp.Domain.Entities.User;
using DatingApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Data.Repositories
{
    public class UserRepository (
        DatingAppDbContext dbContext, IMapper mapper): IUserRepository
    {
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await dbContext.Users.ToListAsync();
        }

        public async Task<IEnumerable<MemberDTO>> GetAllUsersMemberDTO()
        {
            return await dbContext.Users.ProjectTo<MemberDTO>(mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<MemberDTO> GetMemberDTOById(int userId)
        {
            return await dbContext.Users.ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<MemberDTO> GetMemberDTOByUserName(string userName)
        {
            return await dbContext.Users.ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(u => u.UserName.ToLower() == userName.ToLower());
        }

        public async Task<User> GetUserById(int userId)
        {
            return await dbContext.Users.SingleOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User> GetUserByUserName(string userName)
        {
            return await dbContext.Users
                .SingleOrDefaultAsync(u => u.UserName.ToLower() == userName.ToLower());
        }

        public async Task<bool> SaveAllAsync()
        {
            return await dbContext.SaveChangesAsync() > 0;
        }

        public void Update(User user)
        {
            dbContext.Users.Update(user);
        }

        public async Task<User> GetUserByUserNameWithPhotos(string userName)
        {
            return await dbContext.Users.Include(u => u.Photos)
                .SingleOrDefaultAsync(u => u.UserName.ToLower() == userName.ToLower());
        }

        public async Task<MemberDTO> GetMemberDTOByUserNameWithPhotos(string userName)
        {
            return await dbContext.Users
                .Include(u => u.Photos)
                .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(u => u.UserName.ToLower() == userName.ToLower());
        }
    }
}
