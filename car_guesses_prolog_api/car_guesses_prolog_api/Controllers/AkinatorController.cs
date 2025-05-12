using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;

namespace car_guesses_prolog_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AkinatorController : ControllerBase
    {
        private static Dictionary<string, List<int>> Sessions = new();

        [HttpGet("start")]
        public IActionResult StartSession([FromQuery] string sessionId)
        {
            if (!Sessions.ContainsKey(sessionId))
                Sessions[sessionId] = new List<int>();

            var result = CallPrologAsk(new List<int>());
            return Ok(new { result });
        }

        [HttpPost("answer")]
        public IActionResult PostAnswer([FromBody] AnswerRequest request)
        {
            if (!Sessions.ContainsKey(request.SessionId))
                Sessions[request.SessionId] = new List<int>();

            Sessions[request.SessionId].Add(request.Answer);

            var result = CallPrologAsk(Sessions[request.SessionId]);
            return Ok(new { result });
        }

        [HttpPost("add-object")]
        public IActionResult PostAddObject([FromBody] AddObjectRequest request)
        {
            var result = CallPrologAdd(request.Name, request.Answers);
            return Ok(new { result });
        }

        // Вызов Prolog: угадывание
        private string CallPrologAsk(List<int> answers)
        {
            string arg = answers.Count == 0 ? "[]" : $"[{string.Join(",", answers)}]";
            string query = $"ask({arg})";
            return RunProlog(query);
        }

        // Вызов Prolog: добавление
        private string CallPrologAdd(string name, List<int> answers)
        {
            string safeName = $"'{name.Replace("'", "\\'")}'";
            string query = $"add_object({safeName}, [{string.Join(",", answers)}])";
            return RunProlog(query);
        }

        // Общий вызов Prolog
        private string RunProlog(string query)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "swipl",
                Arguments = $"-s cars_akinator.pl -g \"{query}, halt.\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrWhiteSpace(error))
                return $"Error: {error.Trim()}";

            return output.Trim();
        }
    }

    public class AnswerRequest
    {
        public string SessionId { get; set; } = string.Empty;
        public int Answer { get; set; }
    }

    public class AddObjectRequest
    {
        public string Name { get; set; } = string.Empty;
        public List<int> Answers { get; set; } = new();
    }
}