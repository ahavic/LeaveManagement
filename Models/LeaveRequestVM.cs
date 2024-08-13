using LeaveManagement.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagement.Models
{
    public class LeaveRequestVM
    {
        public int Id { get; set; }

        [Display(Name ="Employee Name")]
        public EmployeeVM RequestingEmployee { get; set; }
        public string RequestingEmployeeId { get; set; }

        [Display(Name ="Start Date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name ="Return Date")]
        public DateTime EndDate { get; set; }


        public LeaveTypeVM LeaveType { get; set; }
        public int LeaveTypeId { get; set; }

        [Display(Name ="Date Requested")]
        public DateTime DateRequested { get; set; }

        [Display(Name ="Date Actioned")]
        public DateTime DateActioned { get; set; }

        [Display(Name ="Request Comment")]
        [MaxLength(300)]
        public string RequestComments { get; set; }

        [Display(Name ="Approval Status")]
        //approved = true, rejected = false, pending = null
        public bool? Approved { get; set; }

        public EmployeeVM ApprovedBy { get; set; }
        public string ApprovedById { get; set; }

        public bool Cancelled { get; set; }
    }

    public class AdminLeaveRequestViewVM
    {
        [Display(Name = "Total Requests")]
        public int TotalRequests { get; set; }

        [Display(Name = "Approved Requests")]
        public int ApprovedRequests { get; set; }

        [Display(Name = "Pending Requests")]
        public int PendingRequests { get; set; }

        [Display(Name = "Rejected Requests")]
        public int RejectedRequests { get; set; }
        public List<LeaveRequestVM> LeaveRequests { get; set; }
    }

    public class CreateLeaveRequestVM
    {
        [Display(Name ="Start Date")]
        [Required]
        public string StartDate { get; set; }

        [Display(Name ="End Date")]
        [Required]
        public string EndDate { get; set; }

        //used for dropdown list. each item in list is SelectListItem
        public IEnumerable<SelectListItem> LeaveTypes { get; set; }

        [Display(Name ="Leave Type")]
        public int LeaveTypeId { get; set; }

        public string RequestComments { get; set; }
    }

    public class EmployeeLeaveRequestVM
    {
        public List<LeaveAllocationVM> LeaveAllocations { get; set; }
        public List<LeaveRequestVM> LeaveRequests { get; set; }
    }
}
