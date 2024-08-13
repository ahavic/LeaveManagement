using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagement.Data
{
    public class Employee : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EEID { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateHired { get; set; }
    }
}
