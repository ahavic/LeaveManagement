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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeaveManagement.Controllers
{
    [Authorize]
    public class LeaveRequestController : Controller
    {
        private readonly ILeaveRequestRepository _leaveRequestRepo;
        private readonly ILeaveTypeRepository _leaveTypeRepo;
        private readonly ILeaveAllocationRepository _leaveAllocationRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        public LeaveRequestController(
            ILeaveRequestRepository repo,
            ILeaveTypeRepository leaveTypeRepo,
            ILeaveAllocationRepository leaveAllocationRepo,
            IMapper mapper,
            UserManager<Employee> userManager)
        {
            _leaveRequestRepo = repo;
            _leaveTypeRepo = leaveTypeRepo;
            _leaveAllocationRepo = leaveAllocationRepo;
            _mapper = mapper;
            _userManager = userManager;
        }

        [Authorize(Roles = "Administrator")]
        // GET: LeaveRequest
        public async Task<ActionResult> Index()
        {
            var leaveRequests = await _leaveRequestRepo.FindAll();
            var leaveRequestsModel = _mapper.Map<List<LeaveRequestVM>>(leaveRequests);

            var model = new AdminLeaveRequestViewVM
            {
                TotalRequests = leaveRequestsModel.Count,
                ApprovedRequests = 0,
                PendingRequests = 0,
                RejectedRequests = 0,
                LeaveRequests = leaveRequestsModel
            };

            foreach(var m in leaveRequestsModel)
            {
                if (m.Approved == null)
                {
                    model.PendingRequests++;
                }
                else if (m.Approved == true)
                {
                    model.ApprovedRequests++;
                }
                else
                    model.RejectedRequests++;
            }

            //var model = new AdminLeaveRequestViewVM
            //{
            //    TotalRequests = leaveRequestsModel.Count,
            //    ApprovedRequests = leaveRequestsModel.Count(t => t.Approved == true),
            //    PendingRequests = leaveRequestsModel.Count(t => t.Approved == null),
            //    RejectedRequests = leaveRequestsModel.Count(t => t.Approved == false),
            //    LeaveRequests = leaveRequestsModel
            //};

            return View(model);
        }

        // GET: LeaveRequest/Details/5
        public ActionResult Details(int id)
        {

            return View();
        }

        // GET: LeaveRequest/Create
        public async Task<ActionResult> Create()
        {
            var leaveTypes = await _leaveTypeRepo.FindAll();
            //convert the leaveTypes list into an IEnumerable SelectListItem to match our CreateLeaveRequestVM
            var leaveTypeItems = leaveTypes.Select(t => new SelectListItem {
                Text = t.Name,  //what we see rendered
                Value = t.id.ToString()    //the actual value (value is string)
            });

            var model = new CreateLeaveRequestVM
            {
                LeaveTypes = leaveTypeItems
            };

            return View(model);
        }

        // POST: LeaveRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateLeaveRequestVM model)
        {
            try
            {
                var startDate = Convert.ToDateTime(model.StartDate);
                var endDate = Convert.ToDateTime(model.EndDate);


                var leaveTypes = await _leaveTypeRepo.FindAll();
                //convert the leaveTypes list into an IEnumerable SelectListItem to match our CreateLeaveRequestVM
                var leaveTypeItems = leaveTypes.Select(t => new SelectListItem
                {
                    Text = t.Name,  //what we see rendered
                    Value = t.id.ToString()    //the actual value (value is string)
                });

                model.LeaveTypes = leaveTypeItems;

                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Start Date must come before end date");
                    return View(model);
                }

                if(DateTime.Compare(startDate, endDate) > 1)
                {
                    return View(model);
                }

                //get the user (employee) that is currently logged in
                var employee = _userManager.GetUserAsync(User).Result;
                var allocation = await _leaveAllocationRepo.GetLeaveAllocationByEmployeeAndType(employee.Id, model.LeaveTypeId);
                int daysRequested = (int)(endDate - startDate).TotalDays;

                if (daysRequested > allocation.NumberOfDays)
                {
                    ModelState.AddModelError("", "You do not have sufficient days for this request\nNumber of days left: " + allocation.NumberOfDays) ;
                    return View(model);
                }

                var leaveRequestModel = new LeaveRequestVM
                {
                    RequestingEmployeeId = employee.Id,
                    StartDate = startDate,
                    EndDate = endDate,
                    Approved = null,
                    DateRequested = DateTime.Now,
                    DateActioned = DateTime.Now,
                    LeaveTypeId = model.LeaveTypeId,
                    RequestComments = model.RequestComments
                };

                var leaveRequest = _mapper.Map<LeaveRequest>(leaveRequestModel);
                var success = await _leaveRequestRepo.Create(leaveRequest);
                if(!success)
                {
                    ModelState.AddModelError("", "Something went wrong with submitting your record");
                    return View(model);
                }

                //redirect to index of Home controller
                return RedirectToAction(nameof(Index),"Home");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Something went wrong");
                return View(model);
            }
        }

        // GET: LeaveRequest/Administer/5
        public async Task<ActionResult> Administer(int id)
        {
            var leaveRequest = await _leaveRequestRepo.FindById(id);
            var model = _mapper.Map<LeaveRequestVM>(leaveRequest);
            return View(model);
        }

        public async Task<ActionResult> ApproveRequest(int id)
        {
            try
            {
                var admin = await _userManager.GetUserAsync(User);
                var leaveRequest = await _leaveRequestRepo.FindById(id);
                var allocation = await _leaveAllocationRepo.GetLeaveAllocationByEmployeeAndType(leaveRequest.RequestingEmployeeId, leaveRequest.LeaveTypeId);
                int daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;

                allocation.NumberOfDays -= daysRequested;
                leaveRequest.Approved = true;
                leaveRequest.ApprovedById = admin.Id;
                leaveRequest.DateActioned = DateTime.Now;

                await _leaveRequestRepo.Update(leaveRequest);
                await _leaveAllocationRepo.Update(allocation);

                return RedirectToAction(nameof(Index));                
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Something went wrong");
                return RedirectToAction(nameof(Index));
            }      
        }

        public async Task<ActionResult> RejectRequest(int id)
        {
            try
            {
                var admin = await _userManager.GetUserAsync(User);
                var leaveRequest = await _leaveRequestRepo.FindById(id);
                leaveRequest.Approved = false;
                leaveRequest.ApprovedById = admin.Id;
                leaveRequest.DateActioned = DateTime.Now;

                await _leaveRequestRepo.Update(leaveRequest);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Something went wrong");
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<ActionResult> MyLeave()
        {
            var employee = await _userManager.GetUserAsync(User);            
            var leaveAllocations = 
                _mapper.Map<List<LeaveAllocationVM>>(await _leaveAllocationRepo.GetLeaveAllocationsByEmployee(employee.Id));
            var leaveRequests =
                _mapper.Map<List<LeaveRequestVM>>(await _leaveRequestRepo.GetLeaveRequestsByEmployee(employee.Id));


            EmployeeLeaveRequestVM model = new EmployeeLeaveRequestVM
            {
                LeaveAllocations = leaveAllocations,
                LeaveRequests = leaveRequests
            }; 

            return View(model);
        }

        public async Task<ActionResult> CancelRequest(int id)
        {
            var leaveRequest = await _leaveRequestRepo.FindById(id);
            leaveRequest.Cancelled = true;
            await _leaveRequestRepo.Update(leaveRequest);
            return RedirectToAction("MyLeave");
        }

        // POST: LeaveRequest/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveRequest/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveRequest/Delete/5
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