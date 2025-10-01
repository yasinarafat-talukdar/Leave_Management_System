using Microsoft.AspNetCore.Identity;


namespace LeaveManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        
        public string? FullName { get; set; }
        public string? ManagerId { get; set; } // optional: assign a manager
    }
}