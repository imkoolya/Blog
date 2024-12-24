using Blog.Data.Models;
using Blog.Data.ViewModels.Article;
using Blog.Data.ViewModels.User;
using Blog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private static readonly NLog.ILogger Logger = LogManager.GetCurrentClassLogger();

        public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserViewModel>>> Index()
        {
            var users = _userManager.Users.ToList();
            var userViewModel = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                userViewModel.Add(new UserViewModel
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return Ok(userViewModel);
        }

        [HttpGet("profile")]
        public async Task<ActionResult<ProfileUserViewModel>> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var articles = await _context.Articles.Where(a => a.AuthorId == user.Id).Select(a => new ArticleViewModel
            {
                Id = a.Id,
                Title = a.Title,
                Summary = a.Summary,
                CreatedAt = a.CreatedAt
            }).ToListAsync();

            var model = new ProfileUserViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles.ToList(),
                Articles = articles
            };

            return Ok(model);
        }

        [HttpPut("edit")]
        public async Task<ActionResult> Edit([FromBody] UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("User not found");
            }

            user.UserName = model.UserName;
            user.Email = model.Email;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                Logger.Info($"Пользователь {user.UserName} изменил данные.");
                return Ok(new { message = "Вы успешно изменили данные." });
            }

            foreach (var error in result.Errors)
            {
                return BadRequest($"Произошла ошибка: {error.Description}");
            }

            return StatusCode(500, "Не удалось сохранить изменения.");
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Администратор")]
        public async Task<ActionResult<List<UserViewModel>>> AdminIndex()
        {
            var users = _userManager.Users.ToList();
            var userViewModel = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModel.Add(new UserViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return Ok(userViewModel);
        }

        [HttpPut("admin/edit/{userId}")]
        [Authorize(Roles = "Администратор")]
        public async Task<ActionResult> AdminEdit(string userId, [FromBody] UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            user.UserName = model.UserName;
            user.Email = model.Email;

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (!model.Roles.Contains("Пользователь"))
            {
                model.Roles.Add("Пользователь");
            }

            var rolesToRemove = currentRoles.Except(model.Roles).Where(r => r != "Пользователь").ToList();
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            var rolesToAdd = model.Roles.Except(currentRoles).ToList();
            await _userManager.AddToRolesAsync(user, rolesToAdd);

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                Logger.Info($"Администратор изменил данные {user.UserName}.");
                return Ok(new { message = "Вы успешно изменили данные пользователя." });
            }

            return StatusCode(500, "Не удалось сохранить изменения.");
        }

        [HttpDelete("admin/delete/{userId}")]
        [Authorize(Roles = "Администратор")]
        public async Task<ActionResult> AdminDelete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Администратор"))
            {
                return BadRequest("Невозможно удалить пользователя с ролью администратора.");
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                Logger.Info($"Администратор удалил {user.UserName}.");
                return Ok(new { message = "Пользователь успешно удалён." });
            }

            return StatusCode(500, "Не удалось удалить пользователя.");
        }
    }
}