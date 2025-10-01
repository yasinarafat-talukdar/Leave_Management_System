using LeaveManagement.Models;


namespace LeaveManagement.Services
{
    public interface ILeaveAllocationService
    {
        Task EnsureUserAllocationsAsync(string userId, int year);
        Task<int> GetAvailableDaysAsync(string userId, int leaveTypeId, int year);
        Task<bool> DeductAsync(string userId, int leaveTypeId, int year, int days);
    }
}