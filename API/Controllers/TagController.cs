using Blog.Data.Models;
using Blog.Data.ViewModels.Tag;
using Blog;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly AppDbContext _context;
        private static readonly NLog.ILogger Logger = LogManager.GetCurrentClassLogger();

        public TagController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _context.Tags
                .Select(tag => new TagViewModel
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    ArticleCount = tag.Articles.Count
                })
                .ToListAsync();

            return Ok(tags);
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Администратор")]
        public IActionResult GetAdminTags()
        {
            var tags = _context.Tags.Select(tag => new TagViewModel
            {
                Id = tag.Id,
                Name = tag.Name
            }).ToList();

            return Ok(tags);
        }

        [HttpPost]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> CreateTag([FromBody] TagViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Name))
            {
                return BadRequest("Название тега не может быть пустым.");
            }

            var tag = new Tag
            {
                Name = model.Name
            };

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            Logger.Info($"Тег {tag.Name} создан.");

            return CreatedAtAction(nameof(GetTags), new { id = tag.Id }, tag);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> EditTag(int id, [FromBody] TagViewModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Name))
            {
                return BadRequest("Название тега не может быть пустым.");
            }

            var tag = await _context.Tags.FindAsync(id);

            if (tag == null)
            {
                return NotFound("Тег не найден.");
            }

            tag.Name = model.Name;
            await _context.SaveChangesAsync();
            Logger.Info($"Тег {tag.Name} изменён.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var tag = await _context.Tags.FindAsync(id);

            if (tag == null)
            {
                return NotFound("Тег не найден.");
            }

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            Logger.Info($"Тег {tag.Name} удалён.");

            return NoContent();
        }
    }
}