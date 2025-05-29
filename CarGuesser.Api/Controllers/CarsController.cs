using Microsoft.AspNetCore.Mvc;
using CarGuesser.Api.Data;
using CarGuesser.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CarGuesser.Api.Controllers
{
    [ApiController]
    [Route("api/cars")]
    public class CarsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CarsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddCar([FromBody] Car car) // добавление машины
        {
            if (string.IsNullOrWhiteSpace(car.Name))
                return BadRequest("Название машины обязательно.");

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return Ok(car);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCar(int id) // получение машины по Id
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
                return NotFound("Машина не найдена.");

            return Ok(car);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCars() // получение всех машин
        {
            var cars = await _context.Cars.ToListAsync();
            return Ok(cars);
        }
    }
}
