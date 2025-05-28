using Microsoft.AspNetCore.Mvc;
using AkinatorQueries;
using System.Threading.Tasks;

namespace CarGuesserApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        [HttpGet("top10cars")]
        public async Task<IActionResult> GetTop10Cars()
        {
            try
            {
                var topCars = await AkinatorQueries.Queries.getTop10CarsAsync();
                return Ok(topCars);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при получении топ-10 автомобилей: {ex.Message}");
            }
        }

        [HttpGet("successrate")]
        public async Task<IActionResult> GetSuccessRate()
        {
            try
            {
                var successRate = await AkinatorQueries.Queries.getSuccessRateAsync();
                return Ok(successRate);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при расчете процента успешных угадываний: {ex.Message}");
            }
        }
    }
}