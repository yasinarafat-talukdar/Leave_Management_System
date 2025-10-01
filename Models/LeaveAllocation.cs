using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LeaveManagement.Models
{
    public class LeaveAllocation
    {
        public int Id { get; set; }


        [Required]
        public string UserId { get; set; } = string.Empty;


        [Required]
        public int LeaveTypeId { get; set; }


        [Range(2000, 2100)]
        public int Year { get; set; } = DateTime.UtcNow.Year;


        [Range(0, 365)]
        public int NumberOfDays { get; set; }


        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }
        public LeaveType? LeaveType { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}