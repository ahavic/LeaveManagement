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
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.Controllers
{
    //authorize only admins for leave type controller actions
    [Authorize(Roles="Administrator")]
    public class LeaveTypesController : Controller
    {
        private readonly ILeaveTypeRepository _repo;
        private readonly IMapper _mapper;

        public LeaveTypesController(ILeaveTypeRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        // GET: LeaveTypes
        public async Task<ActionResult> Index()
        {
            var leaveTypes = await _repo.FindAll();  //find all returns collection, so we have to convert to list to use it in Map()
            //model represents mapped version of the data
            //mapping from list of leave types to list of leave type VM
            var model = _mapper.Map<List<LeaveType>, List<LeaveTypeVM>>(leaveTypes.ToList());
            return View(model);
        }

        // GET: LeaveTypes/Details/5
        public async Task<ActionResult> Details(int id)
        {
            bool exists = await _repo.isExists(id);

            if(!exists)
            {
                return NotFound();
            }

            var leaveType = await _repo.FindById(id);
            var model = _mapper.Map<LeaveTypeVM>(leaveType);

            return View(model);
        }

        // GET: LeaveTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(LeaveTypeVM model)
        {
            try
            {
                // TODO: Add insert logic here
                if(!ModelState.IsValid)
                {
                    return View(model);
                }

                //map the leave type VM model to a type of Leave Type
                var leaveType = _mapper.Map<LeaveType>(model);

                leaveType.DateCreated = DateTime.Now;

                //was the model created?
                var isSuccess = await _repo.Create(leaveType); //when managing the database, we use the actual Types (Not VM)
                if(!isSuccess)
                {
                    ModelState.AddModelError("", "Something Went wrong in creating");
                    return View(model); //when passing model to views, we use VM types
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveTypes/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var exists = await _repo.isExists(id);
            if (!exists)
            {
                return NotFound();
            }

            
            var leaveType = await _repo.FindById(id);
            //map LeaveType to LeaveTypeVM
            var model = _mapper.Map<LeaveTypeVM>(leaveType);
            //when passing model to views, we use VM types
            return View(model);
        }

        // POST: LeaveTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(LeaveTypeVM model)
        {
            try
            {
                // TODO: Add update logic here
                if(!ModelState.IsValid)
                {
                    return View(model);
                }

                var leaveType = _mapper.Map<LeaveType>(model);
                var isSuccess = await _repo.Update(leaveType);

                if(!isSuccess)
                {
                    ModelState.AddModelError("", "Something Went wrong in creating");
                    return View(model); //when passing model to views, we use VM types
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveTypes/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var leaveType = await _repo.FindById(id);

            if (leaveType == null)
            {
                return NotFound();
            }

            var success = await _repo.Delete(leaveType);
            if (!success)
            {
                return BadRequest();
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: LeaveTypes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, LeaveTypeVM model)   
        {
            try
            {
                // TODO: Add delete logic here
                var leaveType = await _repo.FindById(id);

                if(leaveType == null)
                {
                    return NotFound();
                }

                var success = await _repo.Delete(leaveType);
                if(!success)
                {
                    return View(model);
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(model);
            }
        }
    }
}