using Blog.Data.Models;
using Blog.Data.ViewModels.Article;
using Blog.Data.ViewModels.Comment;
using Blog.Data.ViewModels.Tag;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace Blog.Controllers
{
    public class ArticleController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private static readonly NLog.ILogger Logger = LogManager.GetCurrentClassLogger();

        public ArticleController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var articles = _context.Articles.Include(a => a.Author).Include(a => a.Tags).OrderByDescending(a => a.CreatedAt).Select(a => new ArticleViewModel
            {
                Id = a.Id,
                Title = a.Title,
                Summary = a.Summary,
                CreatedAt = a.CreatedAt,
                Author = a.Author.UserName,
                Tags = a.Tags.Select(t => new TagViewModel
                {
                    Name = t.Name
                }).ToList()
            }).ToList();

            return View(articles);
        }

        public async Task<IActionResult> Details(int id)
        {
            var article = await _context.Articles
                .Include(a => a.Author)
                .Include(a => a.Comments)
                    .ThenInclude(c => c.Author)
                .Include(a => a.Tags)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
            {
                TempData["Error"] = "Статья не найдена.";
                return RedirectToAction("Index", "Article");
            }

            var model = new ArticleViewModel
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.Content,
                CreatedAt = article.CreatedAt,
                Author = article.Author.UserName,
                Comments = article.Comments.Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    Content = c.Content,
                    Author = c.Author.UserName,
                    CreatedAt = c.CreatedAt
                }).ToList(),
                Tags = article.Tags.Select(t => new TagViewModel
                {
                    Id = t.Id,
                    Name = t.Name
                }).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var allTags = _context.Tags
                .Select(t => new TagViewModel { Id = t.Id, Name = t.Name })
                .ToList();

            var model = new ArticleCreateEditViewModel
            {
                AllTags = allTags
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ArticleCreateEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var article = new Article
                {
                    Title = model.Title,
                    Summary = model.Summary,
                    Content = model.Content,
                    AuthorId = _userManager.GetUserId(User),
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
                TempData["Success"] = "Статья успешно создана.";
                Logger.Info("Пользователь создал статью.");
                _context.SaveChanges();

                return RedirectToAction("Index", "Article");
            }

            model.AllTags = _context.Tags.Select(t => new TagViewModel { Id = t.Id, Name = t.Name }).ToList();

            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var article = _context.Articles.Include(a => a.Tags).FirstOrDefault(a => a.Id == id);

            if (article == null)
            {
                TempData["Error"] = "Статья не найдена.";

                return RedirectToAction("Index", "Article");
            }

            var model = new ArticleCreateEditViewModel
            {
                Id = article.Id,
                Title = article.Title,
                Summary = article.Summary,
                Content = article.Content,
                SelectedTags = article.Tags.Select(t => t.Id).ToList(),
                AllTags = _context.Tags.Select(t => new TagViewModel { Id = t.Id, Name = t.Name }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ArticleCreateEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var article = _context.Articles.Include(a => a.Tags).FirstOrDefault(a => a.Id == model.Id);

                if (article == null)
                {
                    TempData["Error"] = "Статья не найдена.";

                    return RedirectToAction("Index", "Article");
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

                _context.SaveChanges();
                TempData["Success"] = "Статья успешно изменена.";
                Logger.Info("Пользователь изменил статью.");

                return RedirectToAction("Index", "Article");
            }

            model.AllTags = _context.Tags.Select(t => new TagViewModel { Id = t.Id, Name = t.Name }).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var article = await _context.Articles.FindAsync(id);

            if (article == null)
            {
                TempData["Error"] = "Статья не найдена.";

                return RedirectToAction("Index", "Article");
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || article.AuthorId != currentUser.Id)
            {
                TempData["Error"] = "У вас нет прав для удаления этой статьи.";

                return RedirectToAction("Index", "Article");
            }

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Статья успешно удалена.";
            Logger.Info("Пользователь удалил статью.");

            return RedirectToAction("Profile", "User");
        }
    }
}