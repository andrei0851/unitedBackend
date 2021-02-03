using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace Backend.Payloads
{
    public class PostPayload
    {
        public string UserName { get; set; }        
        public string text { get; set; }
        public string imgLink { get; set; }

    }
}
