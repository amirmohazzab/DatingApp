using DatingApp.Api.Errors;
using DatingApp.Application.Extensions;
using DatingApp.Domain.DTOs;
using DatingApp.Domain.Entities.User;
using DatingApp.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DatingApp.Api.Controllers
{
    [Authorize]
    public class UserLikesController 
        (IUserLikeRepository userLikeRepository, IUserRepository userRepository) : BaseController
    {
        [HttpPost("add-like")]
        public async Task<IActionResult> AddLike([FromQuery] string targetUserName)
        {
            var sourceUserId = User.GetUserId();
            int sourceUserIdg = int.Parse(User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sid)?.Value);

            var targetUser = await userRepository.GetUserByUserName(targetUserName);
            if (targetUser == null) return NotFound("User Not Found");
            if (sourceUserId == targetUser.UserId) return BadRequest("like yourself is not possible");

            var userLike = await userLikeRepository.GetUserLike(sourceUserId, targetUser.UserId);
            if (userLike != null) return BadRequest(new ApiResponse(400, "you already liked this user"));

            await userLikeRepository.AddLike(sourceUserId, targetUser.UserId);
            if (await userLikeRepository.SaveAsync())
                return Ok();
            return BadRequest();
        }

        [HttpGet("get-likes")]
        public async Task<ActionResult<PagedList<MemberDTO>>> GetUserLikes([FromQuery] GetLikeParams getLikeParams)
        {
            var userId = User.GetUserId();
            //int userId = 1009;
            return Ok(await userLikeRepository.GetUserLikes(getLikeParams, userId));
        }

        
    }
}
