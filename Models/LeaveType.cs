using System.ComponentModel.DataAnnotations;


namespace LeaveManagement.Models
{
    public class LeaveType
    {
        public int Id { get; set; }


        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;


        [Range(0, 365)]
        public int DefaultDays { get; set; }


        public bool IsActive { get; set; } = true;
    }
}