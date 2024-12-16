using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
