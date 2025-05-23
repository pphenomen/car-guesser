using Microsoft.AspNetCore.Mvc;
using CarGuesser.Api.Data;
using CarGuesser.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CarGuesser.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnswersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AnswersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: api/answers
    [HttpPost]
    public async Task<IActionResult> AddAnswer([FromBody] SessionAnswer answer)
    {
        _context.SessionAnswers.Add(answer);
        await _context.SaveChangesAsync();
        return Ok(answer);
    }

    // GET: api/answers/session/5
    [HttpGet("session/{sessionId}")]
    public async Task<IActionResult> GetAnswersForSession(int sessionId)
    {
        var answers = await _context.SessionAnswers
            .Where(a => a.GameSessionId == sessionId)
            .ToListAsync();

        return Ok(answers);
    }
}
