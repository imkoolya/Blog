using Blog.Data.ViewModels.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [Authorize(Roles = "Администратор")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();

            return View(roles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                TempData["Error"] = "Название роли не может быть пустым.";

                return RedirectToAction("Index", "Role");
            }

            var roleExists = await _roleManager.RoleExistsAsync(roleName);

            if (roleExists)
            {
                TempData["Error"] = "Роль с таким названием уже существует.";

                return RedirectToAction("Index", "Role");
            }

            await _roleManager.CreateAsync(new IdentityRole(roleName));
            TempData["Success"] = "Роль успешно создана.";

            return RedirectToAction("Index", "Role");
        }

        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                TempData["Error"] = "Эта роль не может быть изменена.";

                return RedirectToAction("Index", "Role");
            }

            var model = new RoleViewModel { Id = role.Id, Name = role.Name };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                TempData["Error"] = "Эта роль не может быть изменена.";

                return RedirectToAction("Index", "Role");
            }

            role.Name = model.Name;
            await _roleManager.UpdateAsync(role);
            TempData["Success"] = "Роль успешно изменена.";

            return RedirectToAction("Index", "Role");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                TempData["Error"] = "Роль не найдена.";

                return RedirectToAction("Index", "Role");
            }

            await _roleManager.DeleteAsync(role);
            TempData["Success"] = "Роль успешно удалена.";

            return RedirectToAction("Index", "Role");
        }
    }
}
