using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Entities.Models
{
    public class Post
    {
        [Key]
        public long Id { get; set; }
        public string UserName { get; set; }
        public string text { get; set; }
        public string imageLink { get; set; }

        public int Likes { get; set; }       


    }
}
