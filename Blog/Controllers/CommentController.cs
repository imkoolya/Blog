using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class CommentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
