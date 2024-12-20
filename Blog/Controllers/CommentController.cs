using Blog.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

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
                TempData["Error"] = "Комментарий не может быть пустым.";

                return RedirectToAction("Details", "Article");
            }

            var user = await _userManager.GetUserAsync(User);

            var comment = new Comment
            {
                Content = content,
                CreatedAt = DateTime.UtcNow,
                AuthorId = user.Id,
                ArticleId = articleId
            };

            _context.Comments.Add(comment);
            TempData["Success"] = "Комментарий добавлен.";
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Article", new { id = articleId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                TempData["Error"] = "Комментарий не найден.";
                return RedirectToAction("Details", "Article", new { id = comment.ArticleId });
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (comment.AuthorId != currentUserId && !User.IsInRole("Модератор") && !User.IsInRole("Администратор"))
            {
                return Forbid();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Комментарий удалён.";

            return RedirectToAction("Details", "Article", new { id = comment.ArticleId });
        }

    }
}