using Microsoft.AspNetCore.Mvc;
using CarGuesser.Api.Data;
using CarGuesser.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CarGuesser.Api.Controllers
{
    [ApiController]
    [Route("api/sessionanswers")]
    public class SessionAnswersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SessionAnswersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddAnswer([FromBody] SessionAnswer answer) // добавление ответа пользователя
        {
            if (answer == null || answer.GameSessionId <= 0)
                return BadRequest("Некорректные данные ответа.");

            _context.SessionAnswers.Add(answer);
            await _context.SaveChangesAsync();

            return Ok(answer);
        }

        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> GetAnswersForSession(int sessionId) // получить ответы по сессии
        {
            var answers = await _context.SessionAnswers
                .Where(a => a.GameSessionId == sessionId)
                .ToListAsync();

            if (answers == null || !answers.Any())
                return NotFound($"Ответы для сессии {sessionId} не найдены.");

            return Ok(answers);
        }
    }
}
