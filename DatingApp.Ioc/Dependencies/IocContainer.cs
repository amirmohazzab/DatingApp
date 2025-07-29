using DatingApp.Application.Services.Implementations;
using DatingApp.Application.Services.Interfaces;
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

            #endregion

            #region Repositories


            #endregion
        }
    }
}
