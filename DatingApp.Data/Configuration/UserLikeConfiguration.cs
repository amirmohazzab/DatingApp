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
    public class UserLikeConfiguration : IEntityTypeConfiguration<UserLike>
    {
        public void Configure(EntityTypeBuilder<UserLike> builder)
        {
            builder.HasKey(u => new { u.SourceUserId, u.TargetUserId });
            builder.HasOne(u => u.SourceUser).WithMany(u => u.SourceUserLikes).HasForeignKey(u => u.SourceUserId);
            builder.HasOne(u => u.TargetUser).WithMany(u => u.TargetUserLikes).HasForeignKey(u => u.TargetUserId);
        }
    }
}
