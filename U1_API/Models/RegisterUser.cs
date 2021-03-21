using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace U1_API.Models
{
    public class RegisterUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
