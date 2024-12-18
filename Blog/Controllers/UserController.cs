using Blog.Data.Models;
using Blog.Data.ViewModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog
{
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
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

            return View(userViewModel);
        }

        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            var model = new ProfileUserViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles.ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);

            var model = new UserViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                user.Id = model.UserId;
                user.UserName = model.UserName;
                user.Email = model.Email;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["Success"] = "Вы успешно изменили данные.";

                    return RedirectToAction("Profile", "User");
                }

                foreach (var error in result.Errors)
                {
                    TempData["Error"] = $"Произошла ошибка, {error.Description}";
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> AdminIndex()
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

            return View(userViewModel);
        }

        [HttpGet]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> AdminEdit(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);

            if (!userRoles.Contains("Пользователь"))
            {
                userRoles.Add("Пользователь");
                await _userManager.AddToRoleAsync(user, "Пользователь");
            }

            var model = new UserViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                AllRoles = allRoles,
                Roles = userRoles.ToList()
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> AdminEdit(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);

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
                TempData["Success"] = "Вы успешно изменили данные пользователя.";
                return RedirectToAction("AdminIndex", "User");
            }

            TempData["Error"] = "Не удалось сохранить изменения.";

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> AdminDelete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Администратор"))
            {
                TempData["Error"] = "Невозможно удалить пользователя с ролью администратора.";

                return RedirectToAction("AdminIndex", "User");
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "Пользователь успешно удалён.";

                return RedirectToAction("AdminIndex", "User");
            }

            foreach (var error in result.Errors)
            {
                TempData["Error"] = $"Произошла ошибка, {error.Description}";
            }
            return RedirectToAction("AdminIndex", "User");
        }
    }
}