using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Entities.Models
{
    public class User
    {
        [Key]
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Gender { get; set; }
        public string Role { get; set; }
        public virtual HashSet<User> Followers { get; set; }
        public virtual HashSet<User> Following { get; set; }


   


    }
}
