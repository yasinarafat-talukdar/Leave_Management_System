using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaveManagement.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }

        // <-- ensure requester is set in controller
        public string RequestingUserId { get; set; } = string.Empty;

        [Required]                    // <-- required for binding/validation
        [Range(1, int.MaxValue)]      // <-- prevents 0 when nothing is selected
        public int LeaveTypeId { get; set; }

        [Required]                    // <-- required dates
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [StringLength(500)]
        public string? Reason { get; set; }

        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;

        public DateTime DateRequested { get; set; } = DateTime.UtcNow;
        public DateTime? DateActioned { get; set; }

        public string? ApprovedById { get; set; }

        [ForeignKey(nameof(RequestingUserId))]
        public ApplicationUser? RequestingUser { get; set; }

        [ForeignKey(nameof(ApprovedById))]
        public ApplicationUser? ApprovedBy { get; set; }

        public LeaveType? LeaveType { get; set; }

        [NotMapped]
        public int DaysRequested => (int)(EndDate.Date - StartDate.Date).TotalDays + 1;
    }
}
