using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace car_guesses_prolog_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AkinatorController : ControllerBase
    {
        private class SessionData
        {
            public List<int> Answers = new();
            public (string X1, string X2, int Correct)? PendingDistinction = null;
        }

        private static Dictionary<string, SessionData> Sessions = new();

        [HttpGet("start")]
        public IActionResult StartSession([FromQuery] string sessionId)
        {
            if (!Sessions.ContainsKey(sessionId))
                Sessions[sessionId] = new SessionData();

            var result = CallPrologAsk(new List<int>());
            return Ok(new { result });
        }

        [HttpPost("answer")]
        public IActionResult PostAnswer([FromBody] AnswerRequest request)
        {
            if (!Sessions.ContainsKey(request.SessionId))
                Sessions[request.SessionId] = new SessionData();

            var session = Sessions[request.SessionId];

            // Обработка ответа на уточняющий вопрос (distinguish)
            if (session.PendingDistinction.HasValue)
            {
                var (X1, X2, Correct) = session.PendingDistinction.Value;
                session.PendingDistinction = null;

                if (request.Answer == Correct)
                    return Ok(new { car = X2, isUnique = true });
                else
                    return Ok(new { car = X1, isUnique = true });
            }

            // Добавляем ответ в сессию
            session.Answers.Add(request.Answer);

            // Запрос к Prolog
            var result = CallPrologAsk(session.Answers);

            // Если Prolog вернул вопрос distinguish, парсим и сохраняем
            if (result.StartsWith("distinguish:"))
            {
                // Ожидаемый формат: distinguish:X1:X2:Correct:Вопрос:[Опции]
                var match = Regex.Match(result, @"distinguish:([^:]+):([^:]+):(\d+):([^:]+):\[(.*)\]");
                if (match.Success)
                {
                    string x1 = match.Groups[1].Value;
                    string x2 = match.Groups[2].Value;
                    int correct = int.Parse(match.Groups[3].Value);
                    session.PendingDistinction = (x1, x2, correct);

                    // Возвращаем пользователю вопрос и варианты
                    return Ok(new { question = match.Groups[4].Value, options = match.Groups[5].Value.Split(',') });
                }
            }

            // Если Prolog вернул угадывание
            if (result.StartsWith("guess:"))
            {
                var parts = result.Substring(6).Split(':');
                if (parts.Length == 2 && bool.TryParse(parts[1], out bool isUnique))
                {
                    return Ok(new { car = parts[0], isUnique });
                }
                else
                {
                    return Ok(new { car = result.Substring(6), isUnique = false });
                }
            }

            // По умолчанию возвращаем результат
            return Ok(new { result });
        }

        [HttpPost("add-object")]
        public IActionResult PostAddObject([FromBody] AddObjectRequest request)
        {
            var result = CallPrologAdd(request.Name, request.Answers);
            return Ok(new { result });
        }

        [HttpPost("add-object-with-question")]
        public IActionResult PostAddObjectWithQuestion([FromBody] AddObjectWithQuestionRequest request)
        {
            string safeName = $"'{request.Name.Replace("'", "\\'")}'";
            string baseAnswers = $"[{string.Join(",", request.BaseAnswers)}]";
            string question = $"'{request.Question.Replace("'", "\\'")}'";
            string options = $"['{request.Options[0]}','{request.Options[1]}']";
            string query = $"add_object_with_question({safeName}, {baseAnswers}, {question}, {options}, {request.CorrectOption})";
            var result = RunProlog(query);
            return Ok(new { result });
        }

        private string CallPrologAsk(List<int> answers)
        {
            string arg = answers.Count == 0 ? "[]" : $"[{string.Join(",", answers)}]";
            string query = $"ask({arg})";
            return RunProlog(query);
        }

        private string CallPrologAdd(string name, List<int> answers)
        {
            string safeName = $"'{name.Replace("'", "\\'")}'";
            string query = $"add_object({safeName}, [{string.Join(",", answers)}])";
            return RunProlog(query);
        }

        private string RunProlog(string query)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "swipl",
                Arguments = $"-s cars_akinator.pl -g \"(once({query}) -> true ; true), halt.\"",
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

    public class AddObjectWithQuestionRequest
    {
        public string Name { get; set; } = string.Empty;
        public List<int> BaseAnswers { get; set; } = new();
        public string Question { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
        public int CorrectOption { get; set; }
    }
}