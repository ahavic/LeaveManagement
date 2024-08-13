using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LeaveManagement.Contracts;
using LeaveManagement.Data;
using LeaveManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LeaveAllocationController : Controller
    {
        private readonly ILeaveAllocationRepository _LeaveAllocationrepo;
        private readonly ILeaveTypeRepository _leaveTypeRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        public LeaveAllocationController(
            ILeaveAllocationRepository leaveAllocationRepo, 
            ILeaveTypeRepository leaveTypeRepo, 
            IMapper mapper,
            UserManager<Employee> userManager)
        {
            _LeaveAllocationrepo = leaveAllocationRepo;
            _leaveTypeRepo = leaveTypeRepo;
            _mapper = mapper;
            _userManager = userManager;
        }

        // GET: LeaveAllocation
        public async Task<ActionResult> Index()
        {
            var leaveTypes = await _leaveTypeRepo.FindAll();  
            //model represents mapped version of the data
            //mapping from list of leave types to list of leave type VM
            var mappedLeaveTypes = _mapper.Map<List<LeaveType>, List<LeaveTypeVM>>(leaveTypes.ToList()); //find all returns collection, so we have to convert to list to use it in Map()

            //create leave allocation VM holds a list of leave types VM
            var model = new CreateLeaveAllocationVM
            {
                LeaveTypes = mappedLeaveTypes,
                NumberUpdated = 0
            };

            return View(model);
        }

        public async Task<ActionResult> SetLeave(int id)
        {
            var leaveType = await _leaveTypeRepo.FindById(id);
            //get all users that are employees (in employee role)
            var employees = await _userManager.GetUsersInRoleAsync("Employee");

            //for each employee, create an allocation
            foreach (var employee in employees)
            {
                var check = await _LeaveAllocationrepo.CheckAllocation(leaveType.id, employee.Id);

                //skip leave allocation if the employee already has the leave 
                if (check)
                    continue;

                var allocation = new LeaveAllocationVM
                {
                    DateCreated = DateTime.Now,
                    EmployeeId = employee.Id,
                    LeaveTypeId = id,
                    NumberOfDays = leaveType.DefaultDays,
                    Period = DateTime.Now.Year
                };

                var leaveAllocation = _mapper.Map<LeaveAllocation>(allocation);
                await _LeaveAllocationrepo.Create(leaveAllocation);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<ActionResult> ListEmployees()
        {
            //get all users that are employees (in employee role)
            var employees =  await _userManager.GetUsersInRoleAsync("Employee"); 
            var model = _mapper.Map<List<EmployeeVM>>(employees);
            return View(model);
        }

        // GET: LeaveAllocation/Details/5
        public async Task<ActionResult> Details(string id) //pass an employee id (string)
        {
            var employee = _mapper.Map<EmployeeVM>(await _userManager.FindByIdAsync(id));
            var allocations = _mapper
                .Map<List<LeaveAllocationVM>>(await _LeaveAllocationrepo.GetLeaveAllocationsByEmployee(id));

            var model = new ViewAllocationsVM
            {
                Employee = employee,
                LeaveAllocations = allocations,
                Employeeid = employee.Id
            };

            return View(model);
        }

        // GET: LeaveAllocation/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveAllocation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveAllocation/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var leaveAllocation = await _LeaveAllocationrepo.FindById(id);
            var model = _mapper.Map<EditLeaveAllocationVM>(leaveAllocation);
            return View(model);
        }

        // POST: LeaveAllocation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditLeaveAllocationVM model)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return View(model);
                }

                var record = await _LeaveAllocationrepo.FindById(model.id);
                record.NumberOfDays = model.NumberOfDays;
               
                var success = await _LeaveAllocationrepo.Update(record);
                if(!success)
                {
                    ModelState.AddModelError("", "error while saving");
                    return View(model);
                }

                //redirect to Details. since Deatails takes a string id parameter,
                //we pass in a new object of model.EmployeeId as the parameter to be used in Details(string id)
                return RedirectToAction(nameof(Details), new { id = model.EmployeeId});
            }
            catch
            {
                return View(model);
            }
        }

        // GET: LeaveAllocation/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveAllocation/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}