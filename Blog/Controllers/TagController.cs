using Blog.Data.Models;
using Blog.Data.ViewModels.Tag;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace Blog.Controllers
{
    public class TagController : Controller
    {
        private readonly AppDbContext _context;
        private static readonly NLog.ILogger Logger = LogManager.GetCurrentClassLogger();

        public TagController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var tags = await _context.Tags
                .Select(tag => new TagViewModel
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    ArticleCount = tag.Articles.Count
                })
                .ToListAsync();

            return View(tags);
        }


        public IActionResult AdminIndex()
        {
            var tags = _context.Tags.Select(tag => new TagViewModel
            {
                Id = tag.Id,
                Name = tag.Name
            }).ToList();

            return View(tags);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TagViewModel model)
        {
            if (ModelState.IsValid)
            {
                var tag = new Tag
                {
                    Name = model.Name
                };

                _context.Tags.Add(tag);
                _context.SaveChanges();
                TempData["Success"] = "Тег успешно создан.";
                Logger.Info($"Тег {tag.Name} создан.");

                return RedirectToAction("AdminIndex", "Tag");
            }

            return RedirectToAction("Index", "Tag");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var tag = _context.Tags.FirstOrDefault(t => t.Id == id);

            var model = new TagViewModel
            {
                Id = tag.Id,
                Name = tag.Name
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TagViewModel model)
        {
            if (ModelState.IsValid)
            {
                var tag = _context.Tags.FirstOrDefault(t => t.Id == model.Id);

                tag.Name = model.Name;
                _context.SaveChanges();
                TempData["Success"] = "Тег успешно обновлен.";
                Logger.Info($"Тег {tag.Name} изменён.");

                return RedirectToAction("Index", "Tag");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var tag = _context.Tags.FirstOrDefault(t => t.Id == id);

            _context.Tags.Remove(tag);
            _context.SaveChanges();
            TempData["Success"] = "Тег успешно удален.";
            Logger.Info($"Тег {tag.Name} удален.");

            return RedirectToAction("Index", "Tag");
        }
    }
}