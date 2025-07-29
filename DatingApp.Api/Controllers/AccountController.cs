using DatingApp.Application.Services.Interfaces;
using DatingApp.Data.Context;
using DatingApp.Domain.DTOs;
using DatingApp.Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace DatingApp.Api.Controllers
{
    public class AccountController 
        (DatingAppDbContext dbContext,
        ITokenService tokenService): BaseController
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserTokenDTO>> Register([FromBody] RegisterDTO model)
        {
            if (await IsExistUserName(model.userName))
                return BadRequest("User is deplicated");

            using var hmac = new HMACSHA256();

            var user = new User()
            {
                UserName = model.userName,
                PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(model.password)),
                PasswordSalt = hmac.Key
            };

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            return new UserTokenDTO
            {
                userName = user.UserName,
                token = tokenService.CreateToken(user)
            };
        }


        [HttpPost("login")]
        public async Task<ActionResult<UserTokenDTO>> Login([FromBody] LoginDTO model)
        {
           var user = await dbContext.Users
                .SingleOrDefaultAsync(u => u.UserName.ToLower() == model.userName.ToLower());

            if (user == null)
                return BadRequest("UserName not found");

            using var hmac = new HMACSHA256(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(model.password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                    return BadRequest("Password us wrong");
            }

            return new UserTokenDTO
            {
                userName = user.UserName,
                token = tokenService.CreateToken(user)
            };
        }

        private async Task<bool> IsExistUserName(string userName)
        {
            return await dbContext.Users.AnyAsync(u => u.UserName.ToLower() == userName.ToLower());
        }
    }
}
