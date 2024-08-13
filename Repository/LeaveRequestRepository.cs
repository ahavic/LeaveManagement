using LeaveManagement.Contracts;
using LeaveManagement.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagement.Repository
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly ApplicationDbContext _db;

        public LeaveRequestRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<bool> Create(LeaveRequest entity)
        {
            await _db.LeaveRequests.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(LeaveRequest entity)
        {
            _db.LeaveRequests.Remove(entity);
            return await Save();
        }

        public async Task<ICollection<LeaveRequest>> FindAll()
        {
            return await _db.LeaveRequests
                .Include(t => t.RequestingEmployee)
                .Include(t => t.ApprovedBy)
                .Include(t => t.LeaveType)
                .ToListAsync();
        }

        public async Task<LeaveRequest> FindById(int id)
        {
            var leaveRequest = await _db.LeaveRequests
                .Include(t => t.RequestingEmployee)
                .Include(t => t.ApprovedBy)
                .Include(t => t.LeaveType)
                .FirstOrDefaultAsync(t => t.Id == id);
            return leaveRequest;
        }

        public async Task<ICollection<LeaveRequest>> GetLeaveRequestsByEmployee(string employeeid)
        {
            var leaveReuests = await FindAll();
            return leaveReuests
                .Where(t => t.RequestingEmployeeId == employeeid)
                .ToList();  //NOTE** since we are not directly interacting with the DB, we cannot use ToListAsync()
        }

        public async Task<bool> isExists(int id)
        {
            var exists = await _db.LeaveRequests.AnyAsync(t => t.Id == id);
            return exists;
        }

        public async Task<bool> Save()
        {
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update(LeaveRequest entity)
        {
            _db.LeaveRequests.Update(entity);
            return await Save();
        }
    }
}
