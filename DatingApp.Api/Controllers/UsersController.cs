using AutoMapper;
using DatingApp.Api.Errors;
using DatingApp.Application.Extensions;
using DatingApp.Application.Helper;
using DatingApp.Application.Services.Interfaces;
using DatingApp.Data.Context;
using DatingApp.Domain.DTOs;
using DatingApp.Domain.Entities.Photo;
using DatingApp.Domain.Entities.User;
using DatingApp.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DatingApp.Api.Controllers
{
    
    [ServiceFilter(typeof(LogUserActivity))]
    public class UsersController(
        IUserRepository userRepository,
        IMapper mapper,
        IPhotoService photoService) : BaseController
    {
        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<PagedList<MemberDTO>>> GetUsers([FromQuery] UserParams userParams)
        {
            //userParams.currentUserName = HttpContext.User.GetUserName();
            var users = await userRepository.GetAllUsersMemberDTO(userParams);

            return Ok(users);
        }

        [HttpGet("getUserById/{userId:int}")]
        public async Task<ActionResult<MemberDTO>> GetUserById(int userId)
        {
            var user = await userRepository.GetMemberDTOById(userId);

            if (user == null) return NotFound(new ApiResponse(404, "User Not Found"));
            return Ok(user);
        }

        [HttpGet("getUserByUserName/{userName}", Name = "GetUser")]
        public async Task<ActionResult<MemberDTO>> GetUserByUserName(string userName)
        {
            var user = await userRepository.GetMemberDTOByUserName(userName);

            if (user == null) return NotFound(new ApiResponse(404, "User Not Found"));
            return Ok(user);
        }

        [HttpPut("UpdateUser")]
        public async Task<ActionResult<MemberDTO>> UpdateUser([FromBody] MemberUpdateDto model)
        {
            var username = User.GetUserName();
            var member = await userRepository.GetUserByUserNameWithPhotos(username);
            if (member == null) return NotFound(new ApiResponse(404, "Member Not Found"));

            member = mapper.Map(model, member);
            userRepository.Update(member);

            if (await userRepository.SaveAllAsync())
                return Ok(mapper.Map<MemberDTO>(member));

            return BadRequest(new ApiResponse(400));

        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {
            var result = await photoService.AddPhotoAsync(file);
            if (result.Error != null) return BadRequest(new ApiResponse(400, "Operation Failed"));

            var username = HttpContext.User.GetUserName();
            var user = await userRepository.GetUserByUserNameWithPhotos(username);
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                UserId = user.UserId,
                IsMain = user?.Photos?.Count == 0 ? true : false,
            };
            user?.Photos?.Add(photo);
            userRepository.Update(user);

            if (await userRepository.SaveAllAsync())
                return CreatedAtRoute("GetUser", new { userName = user.UserName }, mapper.Map<PhotoDTO>(photo));

            return BadRequest(new ApiResponse(400, "Operation Failed"));
        }

        [HttpPut("SetMainPhoto/{photoId}")]
        public async Task<ActionResult<PhotoDTO>> SetMainPhoto(int photoId)
        {
            var username = HttpContext.User.GetUserName();
            var user = await userRepository.GetUserByUserNameWithPhotos(username);

            if (user == null)
                return NotFound(new ApiResponse(404, "User Not Found"));

            var photo = user.Photos.FirstOrDefault(p => p.PhotoId == photoId);

            if (photo == null)
                return NotFound(new ApiResponse(404, "Photo Not Found"));

            if (photo.IsMain)
                return BadRequest(new ApiResponse(400, "This photo is default photo"));

            var mainPhoto = user.Photos.FirstOrDefault(p => p.IsMain);
            mainPhoto.IsMain = false;
            photo.IsMain = true;

            userRepository.Update(user);
            if(await userRepository.SaveAllAsync()) return Ok(mapper.Map<PhotoDTO>(photo));

            return BadRequest(new ApiResponse(400));
        }

        [HttpDelete("DeletePhoto/{photoId}")]
        public async Task<IActionResult> DeletePhoto(int photoId)
        {
            var username = HttpContext.User.GetUserName();
            var user = await userRepository.GetUserByUserNameWithPhotos(username);

            if (user == null)
                return NotFound(new ApiResponse(404, "User Not Found"));

            var photo = user.Photos.FirstOrDefault(p => p.PhotoId == photoId);

            if (photo == null)
                return NotFound(new ApiResponse(404, "Photo Not Found"));

            if (photo.IsMain)
                return BadRequest(new ApiResponse(400, "This photo is default photo"));

            await photoService.DeletePhotoAsync(photo.PublicId);

            user.Photos.Remove(photo);
            userRepository.Update(user);

            if (await userRepository.SaveAllAsync()) return Ok(mapper.Map<PhotoDTO>(photo));

            return BadRequest(new ApiResponse(400));
        }

        [HttpGet("get-photos/{userName}")]
        public async Task<ActionResult<MemberDTO>> GetAllPhotos(string userName)
        {
            return await userRepository.GetMemberDTOByUserNameWithPhotos(userName);
        }
    }
}
