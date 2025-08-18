using DatingApp.Domain.DTOs;
using DatingApp.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsers();

        Task<PagedList<MemberDTO>> GetAllUsersMemberDTO(UserParams userParams);

        Task<User> GetUserById(int userId);

        Task<MemberDTO> GetMemberDTOById(int userId);

        Task<MemberDTO> GetMemberDTOByUserName(string userName);

        Task<User> GetUserByUserName(string userName);

        void Update(User user);

        Task<bool> SaveAllAsync();

        Task<User> GetUserByUserNameWithPhotos(string userName);

        Task<MemberDTO> GetMemberDTOByUserNameWithPhotos(string userName);
    }
}
