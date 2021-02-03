using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Entities.Models
{
    public class Follow
    {
        public int Id { get; set;}
        
        public string User { get; set; }

        public string Follows { get; set; }

    }
}
