using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class TagController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
