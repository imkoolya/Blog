﻿using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Blog.Controllers
{
    [ApiController]
    [Route("api/blog/[controller]")]  // Уникальный префикс маршрута
    public class ErrorController : ControllerBase
    {
        private static readonly NLog.ILogger Logger = LogManager.GetCurrentClassLogger();

        [HttpGet("error")]
        public IActionResult Error()
        {
            Logger.Error("Произошла ошибка.");
            return StatusCode(500, new { message = "Произошла ошибка. Пожалуйста, попробуйте позже." });
        }

        [HttpGet("notfound")]
        public IActionResult NotFoundError()
        {
            Logger.Error("Ошибка 404 - Не найдено");
            return NotFound(new { message = "Ресурс не найден." });
        }

        [HttpGet("accessdenied")]
        public IActionResult AccessDenied()
        {
            Logger.Error("Ошибка доступа - Доступ запрещен");
            return Forbid();
        }
    }
}
