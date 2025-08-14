using AutoMapper;
using Azure.Identity;
using DatingApp.Api.Errors;
using DatingApp.Application.Services.Interfaces;
using DatingApp.Data.Context;
using DatingApp.Data.Repositories;
using DatingApp.Domain.DTOs;
using DatingApp.Domain.Entities.User;
using DatingApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace DatingApp.Api.Controllers
{
    public class AccountController 
        (ITokenService tokenService,
        IUserRepository userRepository,
        IAccountRepository accountRepository,
        IMapper mapper): BaseController
    {
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserTokenDTO>> Register([FromBody] RegisterDTO model)
        {
            if (await accountRepository.IsExistUserName(model.userName))
                return BadRequest(new ApiResponse(400, model.userName + "User is duplicated"));

            using var hmac = new HMACSHA256();
            var user = mapper.Map<User>(model);
            user.PasswordSalt = hmac.Key;
            user.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(model.password));

            await accountRepository.addUser(user);

            if (await accountRepository.SaveChangesAsync())
            {
                return Ok(new UserTokenDTO
                {
                    userName = user.UserName,
                    token = tokenService.CreateToken(user)
                });
            }
            return BadRequest(new ApiResponse(400, "Save Data Error"));
        }


        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserTokenDTO>> Login([FromBody] LoginDTO model)
        {
            var user = await accountRepository.GetUserByUserNameWithPhoto(model.userName);

            if (user == null)
                return BadRequest(new ApiResponse(400, "UserName not found"));

            using var hmac = new HMACSHA256(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(model.password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                    return BadRequest(new ApiResponse(400, "Password us wrong"));
            }

            return Ok(new UserTokenDTO
            {
                userName = user.UserName,
                token = tokenService.CreateToken(user),
                photoUrl = user?.Photos?.FirstOrDefault(u => u.IsMain)?.Url
            });
        }

        [HttpGet("IsExistUserName/{userName}")]
        public async Task<ActionResult<bool>> IsExistUserName(string userName)
        {
            return await accountRepository.IsExistUserName(userName);
        }
    }
}
