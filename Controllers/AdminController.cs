using LeaveManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace LeaveManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _users;
        private readonly RoleManager<IdentityRole> _roles;
        public AdminController(UserManager<ApplicationUser> users, RoleManager<IdentityRole> roles)
        { _users = users; _roles = roles; }


        public IActionResult Users() => View(_users.Users.ToList());


        [HttpPost]
        public async Task<IActionResult> AddToRole(string userId, string role)
        {
            var user = await _users.FindByIdAsync(userId);
            if (user is not null && await _roles.RoleExistsAsync(role))
                await _users.AddToRoleAsync(user, role);
            return RedirectToAction(nameof(Users));
        }


        [HttpPost]
        public async Task<IActionResult> RemoveFromRole(string userId, string role)
        {
            var user = await _users.FindByIdAsync(userId);
            if (user is not null && await _roles.RoleExistsAsync(role))
                await _users.RemoveFromRoleAsync(user, role);
            return RedirectToAction(nameof(Users));
        }
    }
}