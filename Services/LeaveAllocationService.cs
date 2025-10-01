using LeaveManagement.Data;
using LeaveManagement.Models;
using Microsoft.EntityFrameworkCore;


namespace LeaveManagement.Services
{
    public class LeaveAllocationService : ILeaveAllocationService
    {
        private readonly ApplicationDbContext _db;
        public LeaveAllocationService(ApplicationDbContext db) => _db = db;


        public async Task EnsureUserAllocationsAsync(string userId, int year)
        {
            var types = await _db.LeaveTypes.Where(t => t.IsActive).ToListAsync();
            foreach (var t in types)
            {
                var exists = await _db.LeaveAllocations.AnyAsync(a => a.UserId == userId && a.LeaveTypeId == t.Id && a.Year == year);
                if (!exists)
                {
                    _db.LeaveAllocations.Add(new LeaveAllocation
                    {
                        UserId = userId,
                        LeaveTypeId = t.Id,
                        Year = year,
                        NumberOfDays = t.DefaultDays
                    });
                }
            }
            await _db.SaveChangesAsync();
        }


        public async Task<int> GetAvailableDaysAsync(string userId, int leaveTypeId, int year)
        {
            var alloc = await _db.LeaveAllocations.FirstOrDefaultAsync(a => a.UserId == userId && a.LeaveTypeId == leaveTypeId && a.Year == year);
            return alloc?.NumberOfDays ?? 0;
        }


        public async Task<bool> DeductAsync(string userId, int leaveTypeId, int year, int days)
        {
            var alloc = await _db.LeaveAllocations.FirstOrDefaultAsync(a => a.UserId == userId && a.LeaveTypeId == leaveTypeId && a.Year == year);
            if (alloc is null || alloc.NumberOfDays < days) return false;
            alloc.NumberOfDays -= days;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}