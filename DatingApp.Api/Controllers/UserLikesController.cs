using DatingApp.Api.Errors;
using DatingApp.Application.Extensions;
using DatingApp.Data.Context;
using DatingApp.Domain.DTOs;
using DatingApp.Domain.Entities.User;
using DatingApp.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DatingApp.Api.Controllers
{
    [Authorize]
    public class UserLikesController 
        (IUserLikeRepository userLikeRepository, IUserRepository userRepository, DatingAppDbContext dbContext) : BaseController
    {
        [HttpPost("add-like")]
        [Authorize(Roles = "member")]
        public async Task<IActionResult> AddLike([FromQuery] string targetUserName)
        {
            var sourceUserId = User.GetUserId();

            var targetUser = await userRepository.GetUserByUserName(targetUserName);
            if (targetUser == null) return NotFound("User Not Found");

            if (sourceUserId == targetUser.Id) return BadRequest("like yourself is not possible");

            var userLike = await userLikeRepository.GetUserLike(sourceUserId, targetUser.Id);
            if (userLike != null) return BadRequest(new ApiResponse(400, "you already liked this user"));

            await userLikeRepository.AddLike(sourceUserId, targetUser.Id);
            if (await userLikeRepository.SaveAsync())
                return Ok();
            return BadRequest();
        }

        [HttpGet("get-likes")]
        [Authorize(Roles = "member")]
        public async Task<ActionResult<PagedList<MemberDTO>>> GetUserLikes([FromQuery] GetLikeParams getLikeParams)
        {
            var id = User.GetUserId();
            return Ok(await userLikeRepository.GetUserLikes(getLikeParams, id));
        }

        [HttpDelete("remove-like")]
        [Authorize(Roles = "member")]
        public async Task<IActionResult> RemoveLike([FromQuery] string targetUserName)
        {
            var sourceUserId = User.GetUserId();

            var targetUser = await userRepository.GetUserByUserName(targetUserName);
            if (targetUser == null) return NotFound("User Not Found");
            if (sourceUserId == targetUser.Id) return BadRequest("You can't unlike yourself");

            var userLike = await userLikeRepository.GetUserLike(sourceUserId, targetUser.Id);
            if (userLike == null) return NotFound("You haven't liked this user");

            userLikeRepository.RemoveLike(userLike);

            if (await userLikeRepository.SaveAsync())
                return Ok();

            return BadRequest("Failed to remove like");
        }

        [HttpGet("is-liked")]
        [Authorize(Roles = "member")]
        public async Task<ActionResult<bool>> IsLiked([FromQuery] string targetUserName)
        {
            var sourceUserId = User.GetUserId();
            var targetUser = await userRepository.GetUserByUserName(targetUserName);
            if (targetUser == null) return NotFound("User Not Found");

            var userLike = await userLikeRepository.GetUserLike(sourceUserId, targetUser.Id);
            return userLike != null;
        }

        [HttpGet("{targetUserName}/count")]
        public async Task<ActionResult<int>> GetLikesCount(string targetUserName)
        {
            var targetUser = await userRepository.GetUserByUserName(targetUserName);
            return await dbContext.UserLikes.CountAsync(l => l.TargetUserId == targetUser.Id);
        }
    }
}
