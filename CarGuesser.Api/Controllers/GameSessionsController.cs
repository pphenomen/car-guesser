using Microsoft.AspNetCore.Mvc;
using CarGuesser.Api.Data;
using CarGuesser.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CarGuesser.Api.Controllers
{
    [ApiController]
    [Route("api/sessions")]
    public class GameSessionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GameSessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartSession([FromBody] StartSessionRequest request) // начать сессию
        {
            if (request.UserId <= 0)
                return BadRequest("Некорректный UserId.");

            var session = new GameSession
            {
                UserId = request.UserId,
                StartedAt = DateTime.UtcNow,
                IsSuccess = false
            };

            _context.GameSessions.Add(session);
            await _context.SaveChangesAsync();

            return Ok(session);
        }

        [HttpPost("finish/{id}")]
        public async Task<IActionResult> FinishSession(int id, [FromBody] FinishSessionRequest request) // завершить сессию
        {
            var session = await _context.GameSessions.FindAsync(id);
            if (session == null)
                return NotFound("Сессия не найдена.");

            session.EndedAt = DateTime.UtcNow;
            session.IsSuccess = request.IsSuccess;
            session.GuessedCar = request.GuessedCar;
            session.OwnerName = request.OwnerName;
            session.OwnerClub = request.OwnerClub;
            session.AddedCar = request.AddedCar;

            await _context.SaveChangesAsync();

            return Ok(session);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSession(int id) // получить сессию по Id
        {
            var session = await _context.GameSessions
                .Include(s => s.Answers)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null)
                return NotFound("Сессия не найдена.");

            return Ok(session);
        }
    }

    public class StartSessionRequest
    {
        public int UserId { get; set; }
    }

    public class FinishSessionRequest
    {
        public bool IsSuccess { get; set; }
        public string? GuessedCar { get; set; }
        public string? OwnerName { get; set; }
        public string? OwnerClub { get; set; }
        public string? AddedCar { get; set; }
    }
}
