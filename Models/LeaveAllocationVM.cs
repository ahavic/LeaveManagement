using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagement.Models
{
    public class LeaveAllocationVM
    {
        public int id { get; set; }

        [Display(Name ="Number of Days")]
        public int NumberOfDays { get; set; } //Leave Days Allocated
        public DateTime DateCreated { get; set; }

        //represents calander year
        public int Period { get; set; }

        //NOTE** View Models should reference other view models only, dont reference a Data Model
        //i.e. reference EmployeeVM, not Employee
        public EmployeeVM Employee { get; set; }
        public string EmployeeId { get; set; } //use Employeeid to get Employee object

        public LeaveTypeVM LeaveType { get; set; }
        public int LeaveTypeId { get; set; }
    }

    public class CreateLeaveAllocationVM
    {
        public int NumberUpdated { get; set; }
        public List<LeaveTypeVM> LeaveTypes { get; set; }
    }

    public class ViewAllocationsVM
    {
        public EmployeeVM Employee { get; set; }
        public string Employeeid { get; set; }
        public List<LeaveAllocationVM> LeaveAllocations { get; set; }
    }

    public class EditLeaveAllocationVM
    {
        public int id { get; set; }
        public EmployeeVM Employee { get; set; }

        public string EmployeeId { get; set; } //use Employeeid to get Employee object
        public int NumberOfDays { get; set; }
        public LeaveTypeVM LeaveType { get; set; }
    }
}
