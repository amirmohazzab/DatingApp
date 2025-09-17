using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Domain.Entities.User
{
    public class Group
    {
        public Group() => Connections = new List<Connection>();

        public Group(string name)
        {
            Name = name;
            Connections = new List<Connection>();
        }

        [Key]
        public string Name { get; set; }

        public List<Connection> Connections { get; set; }
    }
}
