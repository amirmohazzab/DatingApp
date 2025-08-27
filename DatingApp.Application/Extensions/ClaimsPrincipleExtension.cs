using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Application.Extensions
{
    public static class ClaimsPrincipleExtension
    {
        public static string GetUserName(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name)?.Value;

        }

        public static int GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            //var userId = claimsPrincipal?.Claims.ToList().FirstOrDefault(x => x.Type == "Sid")?.Value;
            //return int.Parse(userId);
            //return Convert.ToInt32(claimsPrincipal.FindFirst(ClaimTypes.Sid)?.Value);

            string? userId = claimsPrincipal.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sid)?.Value;

            if (!string.IsNullOrWhiteSpace(userId))
                return int.Parse(userId);

            else return default;

        }
    }
}
