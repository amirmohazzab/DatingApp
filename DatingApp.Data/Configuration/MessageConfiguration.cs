using DatingApp.Domain.Entities.Message;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Data.Configuration
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(u => u.MessageId);
            builder.HasOne(u => u.Sender).WithMany(u => u.MessageSent)
                .HasForeignKey(u => u.SenderId).OnDelete(DeleteBehavior.ClientSetNull);
            builder.HasOne(u => u.Receiver).WithMany(u => u.MessageReceived)
                .HasForeignKey(u => u.ReceiverId).OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
