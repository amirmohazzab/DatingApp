using DatingApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Domain.DTOs
{
    public class RegisterDTO
    {
        [Display(Name = "UserName")]
        [Required(ErrorMessage = "Please enter {0}")]
        [MaxLength(50, ErrorMessage = "UserName must not be more than 50 character")]
        [MinLength(3, ErrorMessage = "UserName must not be less than 3 character")]

        public string userName { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Please enter {0}")]
        [MaxLength(20, ErrorMessage = "Password must not be more than 50 character")]
        [MinLength(5, ErrorMessage = "Password must not be less than 3 character")]

        public string password { get; set; }

        [Display(Name = "Age")]
        [Required(ErrorMessage = "Please enter {0}")]
        public int Age { get; set; }

        [Display(Name = "Gender")]
        [Required(ErrorMessage = "Please enter {0}")]
        public GenderEnum Gender { get; set; }

        [Display(Name = "KnownAs")]
        [Required(ErrorMessage = "Please enter {0}")]
        public string KnownAs { get; set; }

        [Display(Name = "Country")]
        [Required(ErrorMessage = "Please enter {0}")]
        public string Country { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "Please enter {0}")]
        public string City { get; set; }
    }
}
