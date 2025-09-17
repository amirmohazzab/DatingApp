using AutoMapper;
using Azure.Identity;
using CloudinaryDotNet.Actions;
using DatingApp.Api.Errors;
using DatingApp.Application.Services.Interfaces;
using DatingApp.Data.Context;
using DatingApp.Data.Repositories;
using DatingApp.Domain.DTOs;
using DatingApp.Domain.Entities.User;
using DatingApp.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DatingApp.Api.Controllers
{
    public class AccountController 
        (ITokenService tokenService,
        IMapper mapper,
        UserManager<User> userManager,
        SignInManager<User> signInManager): BaseController
    {
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserTokenDTO>> Register([FromBody] RegisterDTO model)
        {
            if (await IsExistUserName(model.userName))
                return BadRequest(new ApiResponse(400, model.userName + "User is duplicated"));
            
            //using var hmac = new HMACSHA256();
            //user.PasswordSalt = hmac.Key;
            //user.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(model.password));
            //await accountRepository.addUser(user);

            var user = mapper.Map<User>(model);
            var result = await userManager.CreateAsync(user, model.password);
            
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new ApiResponse(400, "User creation failed: " + errors));
            }

            var createdUser = await userManager.FindByNameAsync(user?.UserName);
            var roleResult = await userManager.AddToRoleAsync(createdUser, "member");
            if (!roleResult.Succeeded) return BadRequest(new ApiResponse(400, "Save Data Error"));

            return Ok(new UserTokenDTO
            {
                Id = createdUser.Id,
                UserName = createdUser.UserName,
                Token = await tokenService.CreateToken(createdUser),
                Gender = createdUser.Gender,
                KnownAs = createdUser.KnownAs
            });
        }


        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserTokenDTO>> Login([FromBody] LoginDTO model)
        {
            var user = await userManager.Users.Include(u => u.Photos)
                .FirstOrDefaultAsync(u => u.UserName == model.userName);
            if (user == null) return BadRequest(new ApiResponse(400, "UserName Not Found"));

            //using var hmac = new HMACSHA256(user.PasswordSalt);
            //var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(model.password));

            //for (int i = 0; i < computedHash.Length; i++)
            //{
            //    if (computedHash[i] != user.PasswordHash[i])
            //        return BadRequest(new ApiResponse(400, "Password us wrong"));
            //}
            var result = await signInManager.CheckPasswordSignInAsync(user, model.password, false);
            if (!result.Succeeded) return BadRequest(new ApiResponse(400, "UserName or Password Are Wrong"));
            return Ok(new UserTokenDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Token = await tokenService.CreateToken(user),
                PhotoUrl = user?.Photos?.FirstOrDefault(u => u.IsMain)?.Url,
                Gender = user.Gender,
                KnownAs = user.KnownAs,
            });
        }

        [HttpGet("IsExistUserName/{userName}")]
        public async Task<bool> IsExistUserName(string userName)
        {
            return await userManager.Users.AnyAsync(u => u.UserName == userName.ToLower());
        }
    }
}
