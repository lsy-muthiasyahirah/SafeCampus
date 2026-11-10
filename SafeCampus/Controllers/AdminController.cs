
using Microsoft.AspNetCore.Mvc;

namespace SafeCampus.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
