using System;
using System.Linq;
using System.Threading.Tasks;
using LeaveManagement.Data;
using LeaveManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILeaveAllocationService _allocation;

        public LeaveRequestService(ApplicationDbContext db, ILeaveAllocationService allocation)
        {
            _db = db;
            _allocation = allocation;
        }

        public async Task<LeaveRequest> CreateAsync(LeaveRequest request)
        {
            // basic validation
            if (request.EndDate < request.StartDate)
                throw new InvalidOperationException("End date must be after start date");

            // ensure allocations exist for the requester/year
            await _allocation.EnsureUserAllocationsAsync(request.RequestingUserId, request.StartDate.Year);

            var available = await _allocation.GetAvailableDaysAsync(
                request.RequestingUserId,
                request.LeaveTypeId,
                request.StartDate.Year
            );

            if (available < request.DaysRequested)
                throw new InvalidOperationException($"Insufficient allocation. Available: {available}");

            _db.LeaveRequests.Add(request);
            await _db.SaveChangesAsync();
            return request;
        }

        public async Task<bool> ApproveAsync(int requestId, string approverId)
        {
            var req = await _db.LeaveRequests
                .Include(r => r.LeaveType)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (req is null || req.Status != LeaveStatus.Pending)
                return false;

            // deduct allocation; if not enough, fail approval
            var deducted = await _allocation.DeductAsync(
                req.RequestingUserId,
                req.LeaveTypeId,
                req.StartDate.Year,
                req.DaysRequested
            );
            if (!deducted) return false;

            req.Status = LeaveStatus.Approved;
            req.DateActioned = DateTime.UtcNow;
            req.ApprovedById = approverId;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectAsync(int requestId, string approverId)
        {
            var req = await _db.LeaveRequests.FindAsync(requestId);
            if (req is null || req.Status != LeaveStatus.Pending)
                return false;

            req.Status = LeaveStatus.Rejected;
            req.DateActioned = DateTime.UtcNow;
            req.ApprovedById = approverId;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelAsync(int requestId, string userId)
        {
            var req = await _db.LeaveRequests.FindAsync(requestId);
            if (req is null) return false;
            if (req.RequestingUserId != userId) return false;

            if (req.Status == LeaveStatus.Approved)
            {
                // return days back when cancelling an approved request
                var days = (int)(req.EndDate.Date - req.StartDate.Date).TotalDays + 1;
                var alloc = await _db.LeaveAllocations.FirstOrDefaultAsync(a =>
                    a.UserId == userId &&
                    a.LeaveTypeId == req.LeaveTypeId &&
                    a.Year == req.StartDate.Year
                );
                if (alloc != null)
                    alloc.NumberOfDays += days;
            }

            req.Status = LeaveStatus.Cancelled;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
