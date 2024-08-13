using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagement.Models
{
    //VM view model
    public class LeaveTypeVM
    {
        public int id { get; set; } 

        [Display(Name="Leave Type")]
        [Required]
        public string Name { get; set; } 

        [Required]
        [Range(1,120,ErrorMessage = "range must be between 1 and 120")]
        [Display(Name="Default Number of Days")]
        public int DefaultDays { get; set; }

        //NOTE** make DateCreated nullable, otherwise the Required attribute from Name will make Date Created Required as well
        [Display(Name="Date Created")]
        public DateTime? DateCreated { get; set; }
    }

}
