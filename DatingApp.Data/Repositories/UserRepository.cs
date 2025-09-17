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

        public async Task<PagedList<MemberDTO>> GetAllUsersMemberDTO(UserParams userParams)
        {
            var query = dbContext.Users.AsNoTracking();
            query = query.Where(u => u.UserName != userParams.currentUserName);
            query = query.Where(u => u.Gender == userParams.Gender);
            query = query.Where(u => u.Age > userParams.MinAge && u.Age < userParams.MaxAge);

            if (userParams.TypeSort == TypeSort.desc)
            {
                query = userParams.OrderBy switch
                {
                    OrderByEnum.lastActive => query.OrderByDescending(u => u.LastActive),
                    OrderByEnum.created => query.OrderByDescending(u => u.Created),
                    OrderByEnum.age => query.OrderByDescending(u => u.Age),
                    _ => query.OrderByDescending(u => u.LastActive),
                };
            }
            else
            {
                query = userParams.OrderBy switch
                {
                    OrderByEnum.lastActive => query.OrderBy(u => u.LastActive),
                    OrderByEnum.created => query.OrderBy(u => u.Created),
                    OrderByEnum.age => query.OrderBy(u => u.Age),
                    _ => query.OrderBy(u => u.LastActive),
                };
            }
            var result = query.ProjectTo<MemberDTO>(mapper.ConfigurationProvider);
            var items = await PagedList<MemberDTO>.CreateAsync(result, userParams.PageNumber, userParams.PageSize);
            return items;
        }

        public async Task<MemberDTO> GetMemberDTOById(int id)
        {
            return await dbContext.Users.ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(u => u.Id == id);
        }

        public async Task<MemberDTO> GetMemberDTOByUserName(string userName)
        {
            return await dbContext.Users.ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(u => u.UserName.ToLower() == userName.ToLower());
        }

        public async Task<User> GetUserById(int id)
        {
            return await dbContext.Users.SingleOrDefaultAsync(u => u.Id == id);
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
