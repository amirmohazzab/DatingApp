using DatingApp.Domain.DTOs;
using DatingApp.Domain.Entities.User;
using DatingApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Domain.Interfaces
{
    public interface IUserLikeRepository
    {
        Task<UserLike> GetUserLike(int sourceId, int targetId);

        Task<User> GetUserWithLikes(int id);

        Task<PagedList<MemberDTO>> GetUserLikes(GetLikeParams getLikeParams, int id);

        Task AddLike(int sourceId, int targetId);

        Task<bool> SaveAsync();

        void RemoveLike(UserLike like);
    }
}
