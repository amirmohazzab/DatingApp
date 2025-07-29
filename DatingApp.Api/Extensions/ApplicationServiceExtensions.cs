using DatingApp.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace DatingApp.Api.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DatingAppConnectionString");

            services.AddDbContext<DatingAppDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            return services;
        }
    }
}
