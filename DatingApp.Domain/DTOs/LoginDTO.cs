using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Domain.DTOs
{
    public class LoginDTO
    {
        [Display(Name = "UserName")]
        [Required(ErrorMessage = "Please enter UserName")]
        [MaxLength(50, ErrorMessage = "UserName must not be more than 50 character")]
        [MinLength(3, ErrorMessage = "UserName must not be less than 3 character")]

        public string userName { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Please enter Password")]
        [MaxLength(20, ErrorMessage = "Password must not be more than 50 character")]
        [MinLength(5, ErrorMessage = "Password must not be less than 3 character")]

        public string password { get; set; }
    }
}
