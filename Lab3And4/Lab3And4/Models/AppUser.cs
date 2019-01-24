using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Lab3And4.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string LastName { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Surname, FirstName, LastName);
        }
    }
}
