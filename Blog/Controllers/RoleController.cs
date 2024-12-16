using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class RoleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
