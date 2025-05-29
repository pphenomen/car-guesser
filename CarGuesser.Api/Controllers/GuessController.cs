using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CarGuesser.Api.Controllers
{
    [ApiController]
    [Route("api/guess")]
    public class GuessController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public GuessController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("PrologApi");
        }

        [HttpPost("answer")] // принимает ответ пользователя и отправляет его в пролог, возвращает следующий вопрос или результат
        public async Task<IActionResult> Answer([FromBody] AnswerRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("answer", content);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Ошибка вызова Prolog API");

            var responseBody = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseBody);
            var root = doc.RootElement;

            if (root.TryGetProperty("question", out var question))
            {
                return Ok(new { type = "question", text = question.GetString() });
            }

            if (root.TryGetProperty("car", out var car))
            {
                bool isUnique = false;
                if (root.TryGetProperty("isUnique", out var uniqueProp))
                    isUnique = uniqueProp.GetBoolean();

                return Ok(new
                {
                    type = "guess",
                    car = car.GetString(),
                    isUnique = isUnique
                });
            }

            if (root.TryGetProperty("result", out var result))
            {
                var resultStr = result.GetString();
                if (resultStr == "not found")
                {
                    return Ok(new { type = "not_found" });
                }
                else
                {
                    return Ok(new { type = "result", text = resultStr });
                }
            }

            return Ok(new { type = "unknown", text = "Неизвестный ответ от сервера" });
        }

        [HttpGet("start")] // инициализирует новую игровую сессию, возвращает стартовый вопрос
        public async Task<IActionResult> StartSession([FromQuery] string sessionId)
        {
            var response = await _httpClient.GetAsync($"start?sessionId={sessionId}");

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Ошибка вызова Prolog API");

            var responseBody = await response.Content.ReadAsStringAsync();
            return Ok(responseBody);
        }

        [HttpPost("add-object")] // добавляет новый объект с ответами в пролог
        public async Task<IActionResult> AddObject([FromBody] AddObjectRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("add-object", content);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Ошибка вызова Prolog API");

            var responseBody = await response.Content.ReadAsStringAsync();
            return Ok(responseBody);
        }

        [HttpPost("add-object-with-question")] // добавляет новый объект с вопросом для различия в пролог
        public async Task<IActionResult> AddObjectWithQuestion([FromBody] AddObjectWithQuestionRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("add-object-with-question", content);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Ошибка вызова Prolog API");

            var responseBody = await response.Content.ReadAsStringAsync();
            return Ok(responseBody);
        }
    }

    public class AnswerRequest
    {
        public string SessionId { get; set; } = "string";
        public int Answer { get; set; }
    }

    public class AddObjectRequest
    {
        public string Name { get; set; } = "";
        public int[] Answers { get; set; } = new int[0];
    }

    public class AddObjectWithQuestionRequest
    {
        public string Name { get; set; } = "";
        public int[] BaseAnswers { get; set; } = new int[0];
        public string Question { get; set; } = "";
        public string[] Options { get; set; } = new string[0];
        public int CorrectOption { get; set; }
    }
}
