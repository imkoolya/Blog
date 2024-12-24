using Blog.Data.Models;
using Blog.Data.ViewModels.Article;
using Blog;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private static readonly NLog.ILogger Logger = LogManager.GetCurrentClassLogger();

        public ArticleController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            Logger.Info("Получение списка статей.");
            var articles = _context.Articles
                .Include(a => a.Author)
                .Include(a => a.Tags)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.Summary,
                    a.CreatedAt,
                    Author = a.Author.UserName,
                    Tags = a.Tags.Select(t => new { t.Id, t.Name })
                })
                .ToList();

            return Ok(articles);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            Logger.Info($"Получение деталей статьи с ID: {id}.");
            var article = await _context.Articles
                .Include(a => a.Author)
                .Include(a => a.Comments).ThenInclude(c => c.Author)
                .Include(a => a.Tags)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
            {
                Logger.Warn($"Статья с ID {id} не найдена.");
                return NotFound(new { Message = "Статья не найдена." });
            }

            return Ok(new
            {
                article.Id,
                article.Title,
                article.Content,
                article.CreatedAt,
                Author = article.Author.UserName,
                Comments = article.Comments.Select(c => new
                {
                    c.Id,
                    c.Content,
                    c.CreatedAt,
                    Author = c.Author.UserName
                }),
                Tags = article.Tags.Select(t => new { t.Id, t.Name })
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ArticleCreateEditViewModel model)
        {
            Logger.Info("Создание новой статьи.");
            if (!ModelState.IsValid)
            {
                Logger.Warn("Ошибка валидации при создании статьи.");
                return BadRequest(ModelState);
            }

            var currentUserId = _userManager.GetUserId(User);
            var article = new Article
            {
                Title = model.Title,
                Summary = model.Summary,
                Content = model.Content,
                AuthorId = currentUserId,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var tagId in model.SelectedTags)
            {
                var tag = _context.Tags.FirstOrDefault(t => t.Id == tagId);
                if (tag != null)
                {
                    article.Tags.Add(tag);
                }
            }

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();
            Logger.Info($"Статья '{article.Title}' успешно создана.");

            return CreatedAtAction(nameof(GetDetails), new { id = article.Id }, article);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Edit(int id, [FromBody] ArticleCreateEditViewModel model)
        {
            Logger.Info($"Редактирование статьи с ID: {id}.");
            if (!ModelState.IsValid)
            {
                Logger.Warn("Ошибка валидации при редактировании статьи.");
                return BadRequest(ModelState);
            }

            var article = await _context.Articles.Include(a => a.Tags).FirstOrDefaultAsync(a => a.Id == id);
            if (article == null)
            {
                Logger.Warn($"Статья с ID {id} не найдена.");
                return NotFound(new { Message = "Статья не найдена." });
            }

            article.Title = model.Title;
            article.Summary = model.Summary;
            article.Content = model.Content;
            article.Tags.Clear();

            foreach (var tagId in model.SelectedTags)
            {
                var tag = _context.Tags.FirstOrDefault(t => t.Id == tagId);
                if (tag != null)
                {
                    article.Tags.Add(tag);
                }
            }

            await _context.SaveChangesAsync();
            Logger.Info($"Статья с ID {id} успешно обновлена.");

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            Logger.Info($"Удаление статьи с ID: {id}.");
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                Logger.Warn($"Статья с ID {id} не найдена.");
                return NotFound(new { Message = "Статья не найдена." });
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || article.AuthorId != currentUser.Id)
            {
                Logger.Warn($"Пользователь не авторизован или не владеет статьей с ID {id}.");
                return Forbid();
            }

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            Logger.Info($"Статья с ID {id} успешно удалена.");

            return NoContent();
        }
    }
}