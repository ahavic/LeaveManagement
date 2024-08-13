using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagement.Data
{
    public class LeaveType
    {
        [Key]
        public int id { get; set; } //primary key to Table
        [Required]  //null unallowed
        public string Name { get; set; } //type of leave
        public int DefaultDays { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
