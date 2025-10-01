using LeaveManagement.Data;
using LeaveManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace LeaveManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LeaveTypesController : Controller
    {
        private readonly ApplicationDbContext _db;
        public LeaveTypesController(ApplicationDbContext db) => _db = db;


        public async Task<IActionResult> Index()
        => View(await _db.LeaveTypes.OrderBy(t => t.Name).ToListAsync());


        public IActionResult Create() => View(new LeaveType());


        [HttpPost]
        public async Task<IActionResult> Create(LeaveType model)
        {
            if (!ModelState.IsValid) return View(model);
            _db.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var t = await _db.LeaveTypes.FindAsync(id);
            return t is null ? NotFound() : View(t);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(LeaveType model)
        {
            if (!ModelState.IsValid) return View(model);
            _db.Update(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            var t = await _db.LeaveTypes.FindAsync(id);
            return t is null ? NotFound() : View(t);
        }


        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var t = await _db.LeaveTypes.FindAsync(id);
            if (t != null) _db.LeaveTypes.Remove(t);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}