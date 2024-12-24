using Blog.Data.Models;
using Blog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private static readonly NLog.ILogger Logger = LogManager.GetCurrentClassLogger();

        public CommentController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Add(int articleId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                Logger.Warn("Попытка добавить пустой комментарий.");
                return BadRequest(new { Message = "Комментарий не может быть пустым." });
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                Logger.Warn("Пользователь не авторизован для добавления комментария.");
                return Unauthorized(new { Message = "Необходима авторизация." });
            }

            var comment = new Comment
            {
                Content = content,
                CreatedAt = DateTime.UtcNow,
                AuthorId = user.Id,
                ArticleId = articleId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            Logger.Info($"Пользователь {user.UserName} добавил комментарий к статье.");

            return CreatedAtAction(nameof(GetComment), new { id = comment.Id }, comment);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetComment(int id)
        {
            var comment = await _context.Comments
                .Include(c => c.Author)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
            {
                Logger.Warn($"Комментарий с ID {id} не найден.");
                return NotFound(new { Message = "Комментарий не найден." });
            }

            return Ok(new
            {
                comment.Id,
                comment.Content,
                comment.CreatedAt,
                Author = comment.Author.UserName
            });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                Logger.Warn($"Комментарий с ID {id} не найден.");
                return NotFound(new { Message = "Комментарий не найден." });
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (comment.AuthorId != currentUserId && !User.IsInRole("Модератор") && !User.IsInRole("Администратор"))
            {
                Logger.Warn($"Пользователь с ID {currentUserId} не имеет прав на удаление комментария с ID {id}.");
                return Forbid();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            Logger.Info($"Комментарий с ID {id} успешно удалён.");

            return NoContent();
        }
    }
}