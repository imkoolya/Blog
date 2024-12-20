using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult NotFound()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GenericError()
        {
            return View();
        }

        [Route("Error/{statusCode}")]
        public IActionResult HandleError(int statusCode)
        {
            return statusCode switch
            {
                404 => RedirectToAction("NotFound"),
                403 => RedirectToAction("AccessDenied"),
                _ => RedirectToAction("GenericError"),
            };
        }

    }
}
