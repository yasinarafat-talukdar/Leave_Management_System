using LeaveManagement.Models;

namespace LeaveManagement.Services
{
    public interface ILeaveRequestService
    {
        Task<LeaveRequest> CreateAsync(LeaveRequest request);
        Task<bool> ApproveAsync(int requestId, string approverId);
        Task<bool> RejectAsync(int requestId, string approverId);
        Task<bool> CancelAsync(int requestId, string userId);
    }
}
