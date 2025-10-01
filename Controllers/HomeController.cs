using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace LeaveManagement.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index() => View();
    }
}