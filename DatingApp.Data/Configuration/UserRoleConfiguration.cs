using DatingApp.Domain.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Data.Configuration
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.HasKey(ur => new { ur.UserId, ur.RoleId });
            builder.HasOne(u => u.User).WithMany(ur => ur.UserRoles).HasForeignKey(u => u.UserId);
            builder.HasOne(r => r.Role).WithMany(ur => ur.UserRoles).HasForeignKey(u => u.RoleId);
        }
    }
}
