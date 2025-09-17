using DatingApp.Data.Context;
using DatingApp.Domain.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatingApp.Data.SeedData
{
    public class SeedUserData
    {
        public static async Task SeedUsers(DatingAppDbContext dbcontext, ILoggerFactory loggerFactory, UserManager<User> userManager)
        {
            try
            {
                if (!await dbcontext.Users.AnyAsync())
                {
                    var userData = await File.ReadAllTextAsync("UserSeedData.json");
                    var users = JsonSerializer.Deserialize<List<User>>(userData);
                    if (users == null) return;
                    foreach (var user in users)
                    {
						  await userManager.CreateAsync(user, "P@$$w0rd");
                    //    using var hmac = new HMACSHA512();
                    //    user.UserName = user.UserName.ToLower();
                    //    user.PasswordSalt = hmac.Key;
                    //    user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                    //    //await userManager.CreateAsync(user, "P@$$w0rd");
                    }
                    //await dbcontext.Users.AddRangeAsync(users);
                    await dbcontext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var log = loggerFactory.CreateLogger<SeedUserData>();
                log.LogError(ex.Message);
            }
        }
    }


}
