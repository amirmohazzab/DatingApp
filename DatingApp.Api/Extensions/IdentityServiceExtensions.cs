using CloudinaryDotNet.Actions;
using DatingApp.Data.Context;
using DatingApp.Domain.Entities.User;
using DatingApp.Domain.Entities.Role;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DatingApp.Api.Extensions
{
    public static class IdentityServiceExtension
    {
        public static IServiceCollection AddIdentityService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentityCore<User>(options =>
            {
                //opt.Password.RequireDigit = true;
            }).AddRoles<Domain.Entities.Role.Role>()
              .AddUserManager<Microsoft.AspNetCore.Identity.UserManager<User>>()
              .AddRoleManager<Microsoft.AspNetCore.Identity.RoleManager<Domain.Entities.Role.Role>>()
              .AddSignInManager<SignInManager<User>>()
              .AddRoleValidator<Microsoft.AspNetCore.Identity.RoleValidator<Domain.Entities.Role.Role>>()
              .AddEntityFrameworkStores<DatingAppDbContext>();


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:Key"]))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization(opt =>
            {
                opt.AddPolicy("RequireAdminRole", policy => policy.RequireRole("admin", "member"));
            });

            return services;
        }

    }
}
