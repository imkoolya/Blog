using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace API.Controllers
{
    [Authorize(Roles = "Администратор")]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private static readonly NLog.ILogger Logger = LogManager.GetCurrentClassLogger();

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult GetRoles()
        {
            var roles = _roleManager.Roles.ToList();
            Logger.Info("Администратор запросил список ролей.");

            return Ok(roles);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                return BadRequest("Название роли не может быть пустым.");
            }

            var roleExists = await _roleManager.RoleExistsAsync(role);

            if (roleExists)
            {
                return Conflict("Роль с таким названием уже существует.");
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(role));

            if (result.Succeeded)
            {
                Logger.Info($"Администратор создал роль {role}.");
                return CreatedAtAction(nameof(GetRoles), new { role }, role);
            }
            else
            {
                return StatusCode(500, "Ошибка при создании роли.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound("Роль не найдена.");
            }

            return Ok(role);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditRole(string id, [FromBody] string newRoleName)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound("Роль не найдена.");
            }

            role.Name = newRoleName;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                Logger.Info($"Администратор изменил роль с ID {id} на {newRoleName}.");
                return NoContent();
            }
            else
            {
                return StatusCode(500, "Ошибка при обновлении роли.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound("Роль не найдена.");
            }

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                Logger.Info($"Администратор удалил роль с ID {id}.");
                return NoContent();
            }
            else
            {
                return StatusCode(500, "Ошибка при удалении роли.");
            }
        }
    }
}