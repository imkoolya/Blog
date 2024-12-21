using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using NLog;

    public class ErrorController : Controller
    {
        private static readonly NLog.ILogger Logger = LogManager.GetCurrentClassLogger();

        [Route("Error")]
        public IActionResult Error()
        {
            Logger.Error("Произошла ошибка.");

            return View("Error");
        }

        [Route("NotFound")]
        public IActionResult NotFound()
        {
            Logger.Error("Произошла ошибка");

            return View("NotFound");
        }

        [Route("AccessDenied")]
        public IActionResult AccessDenied()
        {
            Logger.Error("Произошла ошибка");

            return View("AccessDenied");
        }
    }
}
