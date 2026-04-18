using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WEB.Controllers
{
    [Authorize(Policy = "Admins")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}