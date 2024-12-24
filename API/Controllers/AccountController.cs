using Blog.Data.Models;
using Blog.Data.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private static readonly NLog.ILogger Logger = LogManager.GetCurrentClassLogger();

        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = model.UserName, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Пользователь");

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    Logger.Info($"Пользователь {user.UserName} зарегистрировался.");

                    return Ok(new { message = "Пользователь успешно зарегистрирован." });
                }

                foreach (var error in result.Errors)
                {
                    Logger.Error($"Ошибка регистрации: {error.Description}");
                }

                return BadRequest(new { message = "Ошибка регистрации", errors = result.Errors });
            }

            return BadRequest(new { message = "Некорректные данные для регистрации." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return Unauthorized(new { message = "Неверные данные для входа." });
                }

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    Logger.Info($"Пользователь {user.UserName} вошел в аккаунт.");

                    return Ok(new { message = "Вы успешно вошли в аккаунт." });
                }

                return Unauthorized(new { message = "Неверные данные для входа." });
            }

            return BadRequest(new { message = "Некорректные данные для входа." });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            Logger.Info("Пользователь вышел из аккаунта.");

            return Ok(new { message = "Вы успешно вышли из аккаунта." });
        }
    }
}