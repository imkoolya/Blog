using Blog.Data.Models;
using Blog.Data.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Blog.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private static readonly NLog.ILogger Logger = LogManager.GetCurrentClassLogger();

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = model.UserName, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Пользователь");
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    TempData["Success"] = "Пользователь успешно зарегистрирован.";
                    Logger.Info($"Пользователь {user} зарегистрировался.");

                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    Logger.Error($"Произошла ошибка: {error.Description}");
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    TempData["Error"] = "Неудачная попытка входа, проверьте верно ли вы ввели все данные.";

                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Вы успешно вошли в аккаунт.";
                    Logger.Info($"Пользователь {user} зашёл в аккаунт.");

                    return RedirectToAction("Index", "Home");
                }
                TempData["Error"] = "Неудачная попытка входа, проверьте верно ли вы ввели все данные.";
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            return View("Logout");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogoutConfirmed()
        {
            await _signInManager.SignOutAsync();
            TempData["Success"] = "Вы успешно вышли из аккаунта.";
            Logger.Info("Пользователь вышел из аккаунта.");

            return RedirectToAction("Index", "Home");
        }
    }
}