using Microsoft.AspNetCore.Mvc;
using CarGuesser.Api.Data;
using CarGuesser.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CarGuesser.Api.Controllers
{
    [ApiController]
    [Route("api/questions")]
    public class QuestionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuestionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddQuestion([FromBody] Question question) // ���������� �������
        {
            if (string.IsNullOrWhiteSpace(question.Text))
                return BadRequest("����� ������� ����������.");

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return Ok(question);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestion(int id) // ��������� ������� �� Id
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null)
                return NotFound("������ �� ������.");

            return Ok(question);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllQuestions() // ��������� ���� ��������
        {
            var questions = await _context.Questions.ToListAsync();
            return Ok(questions);
        }
    }
}
