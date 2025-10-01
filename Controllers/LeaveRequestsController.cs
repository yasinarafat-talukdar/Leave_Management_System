using System;
using System.Linq;
using System.Threading.Tasks;
using LeaveManagement.Data;
using LeaveManagement.Models;
using LeaveManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Controllers
{
    [Authorize]
    public class LeaveRequestsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILeaveRequestService _service;
        private readonly ApplicationDbContext _db;

        public LeaveRequestsController(
            UserManager<ApplicationUser> userManager,
            ILeaveRequestService service,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _service = service;
            _db = db;
        }

        // EMPLOYEE: My requests list
        public async Task<IActionResult> My()
        {
            var uid = _userManager.GetUserId(User)!;
            var items = await _db.LeaveRequests
                .Include(r => r.LeaveType)
                .Where(r => r.RequestingUserId == uid)
                .OrderByDescending(r => r.DateRequested)
                .ToListAsync();

            return View(items); // Views/LeaveRequests/My.cshtml
        }

        // MANAGER/ADMIN: Manage all requests
        [Authorize(Policy = "CanApprove")]
        public async Task<IActionResult> Index()
        {
            var items = await _db.LeaveRequests
                .Include(r => r.LeaveType)
                .Include(r => r.RequestingUser)
                .OrderByDescending(r => r.DateRequested)
                .ToListAsync();

            return View(items); // Views/LeaveRequests/Index.cshtml
        }

        // GET: Create request form
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.LeaveTypes = await _db.LeaveTypes
                .Where(t => t.IsActive)
                .OrderBy(t => t.Name)
                .ToListAsync();

            return View(new LeaveRequest
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today
            }); // Views/LeaveRequests/Create.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveRequest model)
        {
            var uid = _userManager.GetUserId(User)!;
            model.RequestingUserId = uid;

            if (model.EndDate < model.StartDate)
                ModelState.AddModelError(string.Empty, "End date must be after start date.");

            if (!ModelState.IsValid)
            {
                ViewBag.LeaveTypes = await _db.LeaveTypes
                    .Where(t => t.IsActive)
                    .OrderBy(t => t.Name)
                    .ToListAsync();
                return View(model);
            }

            try
            {
                await _service.CreateAsync(model);
                TempData["msg"] = "Leave request submitted.";
                return RedirectToAction(nameof(My));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.LeaveTypes = await _db.LeaveTypes
                    .Where(t => t.IsActive)
                    .OrderBy(t => t.Name)
                    .ToListAsync();
                return View(model);
            }
        }


        // DETAILS: Any authenticated user can view their own; approvers can view all
        public async Task<IActionResult> Details(int id)
        {
            var req = await _db.LeaveRequests
                .Include(r => r.LeaveType)
                .Include(r => r.RequestingUser)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (req is null) return NotFound();
            // Optional: restrict non-approvers to their own requests
            if (!(User.IsInRole("Admin") || User.IsInRole("Manager")))
            {
                var uid = _userManager.GetUserId(User)!;
                if (req.RequestingUserId != uid) return Forbid();
            }

            return View(req); // Views/LeaveRequests/Details.cshtml
        }

        // APPROVE: Manager/Admin only
        [Authorize(Policy = "CanApprove")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var uid = _userManager.GetUserId(User)!;
            await _service.ApproveAsync(id, uid);
            return RedirectToAction(nameof(Index));
        }

        // REJECT: Manager/Admin only
        [Authorize(Policy = "CanApprove")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var uid = _userManager.GetUserId(User)!;
            await _service.RejectAsync(id, uid);
            return RedirectToAction(nameof(Index));
        }

        // CANCEL: Requester only
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var uid = _userManager.GetUserId(User)!;
            await _service.CancelAsync(id, uid);
            return RedirectToAction(nameof(My));
        }
    }
}
