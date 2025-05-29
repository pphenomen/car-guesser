using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BackendControll.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BeckendController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public BeckendController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("PrologApi");
        }

        [HttpPost("sendanswer")]
        public async Task<IActionResult> SendAnswer([FromBody] AnswerRequest request)
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
                return Ok(new { question = question.GetString() });
            if (root.TryGetProperty("car", out var car))
                return Ok(new { car = car.GetString() });
            if (root.TryGetProperty("result", out var result))
                return Ok(new { result = result.GetString() });

            return Ok(new { message = "Нет данных в ответе" });
        }

        // GET /beckend/start?sessionId=xxx
        [HttpGet("start")]
        public async Task<IActionResult> StartSession([FromQuery] string sessionId)
        {
            var response = await _httpClient.GetAsync($"start?sessionId={sessionId}");

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Ошибка вызова Prolog API");

            var responseBody = await response.Content.ReadAsStringAsync();
            return Ok(responseBody);
        }

        // POST /beckend/add-object
        [HttpPost("add-object")]
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

        // POST /beckend/add-object-with-question
        [HttpPost("add-object-with-question")]
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