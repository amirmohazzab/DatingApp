using DatingApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Domain.Entities.User
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public GenderEnum Gender { get; set; }

        public string UserName { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public string? Email { get; set; }

        public int Age { get; set; }

        public DateTime? LastActive { get; set; }

        public DateTime? Created { get; set; }

        public string? KnownAs { get; set; }

        public string? Introduction { get; set; }

        public string? LookingFor { get; set; }

        public string? Interests { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }

        [InverseProperty("User")]
        public ICollection<Photo.Photo>? Photos { get; set; }
    }
}
