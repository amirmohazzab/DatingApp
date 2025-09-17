using DatingApp.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Domain.Entities.User
{
    public class User : IdentityUser<int>
    {
        public GenderEnum Gender { get; set; }

        public override string? UserName { get; set; }

        public override string? Email { get; set; }

        public int Age { get; set; }

        public DateTime? LastActive { get; set; }

        public DateTime? Created { get; set; } = DateTime.Now;

        public string? KnownAs { get; set; }

        public string? Introduction { get; set; }

        public string? LookingFor { get; set; }

        public string? Interests { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }

        [InverseProperty("User")]
        public ICollection<Photo.Photo>? Photos { get; set; }

        public ICollection<UserLike>? SourceUserLikes { get; set; }

        public ICollection<UserLike>? TargetUserLikes { get; set; }

        public ICollection<Message.Message>? MessageSent { get; set; }

        public ICollection<Message.Message>? MessageReceived { get; set; }

        public ICollection<UserRole>? UserRoles { get; set; }
    }
}
