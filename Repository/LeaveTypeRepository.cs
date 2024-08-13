using LeaveManagement.Contracts;
using LeaveManagement.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagement.Repository
{
    public class LeaveTypeRepository : ILeaveTypeRepository
    {
        private readonly ApplicationDbContext _db;

        public LeaveTypeRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Create(LeaveType entity)
        {
            await _db.LeaveTypes.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(LeaveType entity)
        {
            _db.LeaveTypes.Remove(entity);
            return await Save();
        }

        public async Task<ICollection<LeaveType>> FindAll()
        {
            return await _db.LeaveTypes.ToListAsync();
        }

        public async Task<LeaveType> FindById(int id)
        {
            return await _db.LeaveTypes
                .FindAsync(id);
        }

        public ICollection<LeaveType> GetEmployeesByLeaveType()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> isExists(int id)
        {
            var exists = await _db.LeaveTypes.AnyAsync(t => t.id == id);
            return exists;
        }

        //SaveChanges returns an int
        public async Task<bool> Save()
        {
            return await _db.SaveChangesAsync() > 0; //if a change is made (added, deleted, updated to db), SaveChanges will return an int > 0   
        }

        public async Task<bool> Update(LeaveType entity)
        {
            _db.LeaveTypes.Update(entity);
            return await Save();
        }
    }
}
