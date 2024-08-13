using LeaveManagement.Contracts;
using LeaveManagement.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagement.Repository
{
    public class LeaveAllocationRepository : ILeaveAllocationRepository
    {
        private readonly ApplicationDbContext _db;

        public LeaveAllocationRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> CheckAllocation(int leaveTypeId, string employeeId)
        {
            var period = DateTime.Now.Year;
            var allocation = await FindAll();
            return allocation
                .Where(t => t.EmployeeId == employeeId && t.LeaveTypeId == leaveTypeId && t.Period == period)
                .Any();
        }

        public async Task<bool> Create(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Add(entity);
            return await Save();
        }

        public async Task<bool> Delete(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Remove(entity);
            return await Save();
        }

        public async Task<ICollection<LeaveAllocation>> FindAll()
        {
            //include is like implicit inner join
            //query brings over all possible details for anything that  we include
            var leaveAllocations = await _db.LeaveAllocations
                .Include(t => t.LeaveType)
                .Include(t => t.Employee)
                .ToListAsync();
            return leaveAllocations;
        }

        public async Task<LeaveAllocation> FindById(int id)
        {
            return await _db.LeaveAllocations
                .Include(t => t.LeaveType)
                .Include(t => t.Employee)
                .FirstOrDefaultAsync(t => t.id == id);
        }

        public async Task<LeaveAllocation> GetLeaveAllocationByEmployeeAndType(string id, int leaveTypeId)
        {
            var period = DateTime.Now.Year;
            var allocation = await FindAll();
            return allocation
                .FirstOrDefault(t => t.EmployeeId == id && t.Period == period && t.LeaveTypeId == leaveTypeId);
        }

        public async Task<ICollection<LeaveAllocation>> GetLeaveAllocationsByEmployee(string id)
        {
            var period = DateTime.Now.Year;
            var allocation = await FindAll();
            return allocation
                .Where(t => t.EmployeeId == id && t.Period == period)
                .ToList();
        }

        public async Task<bool> isExists(int id)
        {
            var exists = await _db.LeaveAllocations.AnyAsync(t => t.id == id);
            return exists;
        }

        public async Task<bool> Save()
        {
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update(LeaveAllocation entity)
        {
            _db.LeaveAllocations.Update(entity);
            return await Save();
        }
    }
}
