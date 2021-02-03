using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    public class RegisterPayload
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string Gender { get; set; }


    }
}