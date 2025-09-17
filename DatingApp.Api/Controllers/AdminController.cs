using DatingApp.Domain.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatingApp.Api.Errors;
using DatingApp.Domain.DTOs;
using DatingApp.Data.Context;

namespace DatingApp.Api.Controllers
{
    public class AdminController (DatingAppDbContext dbContext, UserManager<User> userManager) : BaseController
    {
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("GetUsersWithRoles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            var users = await userManager.Users.Include(u => u.UserRoles).ThenInclude(u => u.Role)
                .Select(u => new UserTokenDTO
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList(),
                    Gender = u.Gender,
                    KnownAs = u.KnownAs
                }).ToListAsync();

            return Ok(users);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-role/{userName}")]
        public async Task<IActionResult> EditRole(string userName, [FromQuery] string roles)
        {
            var selectedRoles = roles.Split(",");
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null) return NotFound(new ApiResponse(400, userName + "Not Found"));

            var userRole = dbContext.UserRoles.Where(ur => ur.UserId == user.Id).ToList();
            if (userRole.Any() && userRole.Count() > 0)
            {
                dbContext.UserRoles.RemoveRange(userRole);
                await dbContext.SaveChangesAsync();
            }

            if (selectedRoles.Any() && selectedRoles.Count() > 0)
            {
                var addRes = await userManager.AddToRolesAsync(user, selectedRoles);
                if (!addRes.Succeeded) return BadRequest(new ApiResponse(400, "Add Roll Error"));
                return Ok(selectedRoles);
            }

            return null;
        }
    }
}
