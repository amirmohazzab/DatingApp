using DatingApp.Domain.Entities.Message;
using DatingApp.Domain.Entities.Photo;
using DatingApp.Domain.Entities.Role;
using DatingApp.Domain.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Data.Context
{
    public class DatingAppDbContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, UserRole, 
                                        IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DatingAppDbContext(DbContextOptions<DatingAppDbContext> options) : base(options) { }

        public override DbSet<User> Users { get; set; }

        public DbSet<Photo> Photos { get; set; }

        public DbSet<UserLike> UserLikes { get; set; }

        public DbSet<Message> Messages { get; set; }

        public override DbSet<UserRole> UserRoles { get; set; }

        public override DbSet<Role> Roles { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<Connection> Connections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
