using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string Role { get; set; }
    }
}
