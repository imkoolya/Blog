using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private static readonly NLog.ILogger Logger = LogManager.GetCurrentClassLogger();

        public IActionResult Index()
        {
            Logger.Info("Пользователь зашёл на главную.");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(Roles = "Администратор")]
        public IActionResult AdminPanel()
        {
            Logger.Info("Администратор зашёл в Админ Панель.");
            return View();
        }

        [Authorize(Roles = "Модератор")]
        public IActionResult ModerPanel()
        {
            Logger.Info("Модератор зашёл в Модер Панель.");
            return View();
        }
    }
}
