using DatingApp.Data.Context;
using DatingApp.Domain.Entities.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Controllers
{
    public class UsersController(DatingAppDbContext dbContext) : BaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<List<User>> GetUsers()
        {
            return await dbContext.Users.ToListAsync();
        }

        [HttpGet("{userId}")]
        [Authorize]
        public async Task<User?> GetUser(int userId)
        {
            return await dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }
    }
}
