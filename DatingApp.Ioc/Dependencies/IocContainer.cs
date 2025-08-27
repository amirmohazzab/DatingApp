using AutoMapper;
using DatingApp.Application.Services.Implementations;
using DatingApp.Application.Services.Interfaces;
using DatingApp.Data.Repositories;
using DatingApp.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Ioc.Dependencies
{
    public static class IocContainer
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            #region Services

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPhotoService, PhotoService>();

            #endregion

            #region Repositories

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IUserLikeRepository, UserLikeRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();

            #endregion
        }
    }
}
