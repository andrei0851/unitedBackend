using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Entities.Models
{
    public class Like
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int postID { get; set; }
    }
}
