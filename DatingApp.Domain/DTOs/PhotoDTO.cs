using DatingApp.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Domain.DTOs
{
    public class PhotoDTO
    {
        public int PhotoId { get; set; }

        public string Url { get; set; }

        public bool IsMain { get; set; }
    }
}
