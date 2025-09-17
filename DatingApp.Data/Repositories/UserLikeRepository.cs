using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingApp.Data.Context;
using DatingApp.Domain.DTOs;
using DatingApp.Domain.Entities.User;
using DatingApp.Domain.Enums;
using DatingApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DatingApp.Data.Repositories
{
    public class UserLikeRepository 
        (DatingAppDbContext dbContext,
        IMapper mapper) : IUserLikeRepository
    {
        public async Task AddLike(int sourceId, int targetId)
        {
            await dbContext.UserLikes.AddAsync(new UserLike { SourceUserId = sourceId, TargetUserId = targetId });
        }

        public async Task<UserLike> GetUserLike(int sourceId, int targetId)
        {
            return await dbContext.UserLikes.FindAsync(sourceId, targetId);
        }

        public async Task<PagedList<MemberDTO>> GetUserLikes(GetLikeParams getLikeParams, int id)
        {
            var users = dbContext.Users.AsQueryable();
            var likes = dbContext.UserLikes.AsQueryable();

            if (getLikeParams.PredicateUserLike == PredicateLikeEnum.Liked)
            {
                likes = likes.Include(u => u.TargetUser)
                    .ThenInclude(u => u.Photos)
                    .Where(u => u.SourceUserId == id);

                users = likes.Select(u => u.TargetUser);
            }
            if (getLikeParams.PredicateUserLike == PredicateLikeEnum.LikeBy)
            {
                likes = likes.Include(u => u.SourceUser)
                    .ThenInclude(u => u.Photos)
                    .Where(u => u.TargetUserId == id);

                users = likes.Select(u => u.SourceUser);

            }
            var result = users.ProjectTo<MemberDTO>(mapper.ConfigurationProvider);
            return await PagedList<MemberDTO>.CreateAsync(result, getLikeParams.PageNumber, getLikeParams.PageSize);
        }

        public Task<User> GetUserWithLikes(int id)
        {
            return dbContext.Users.Include(u => u.TargetUserLikes).FirstOrDefaultAsync(u =>  u.Id == id);
        }

        public async Task<bool> SaveAsync()
        {
            return await dbContext.SaveChangesAsync() > 0;
        }

        public void RemoveLike(UserLike like)
        {
            dbContext.UserLikes.Remove(like);
        }
    }
}
