using Microsoft.AspNetCore.Mvc;
using back.Models;
using System.Linq;
using back.DataContext;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ToursController : ControllerBase
{
    private readonly MyDataContext _context;

    public ToursController(MyDataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetTours()
    {
        var tours = _context.Tours.ToList();
        return Ok(tours);
    }

    [HttpGet("{id}")]
    public IActionResult GetTourById(int id)
    {
        var tour = _context.Tours.FirstOrDefault(t => t.Id == id);
        if (tour == null)
        {
            return NotFound($"Tour with ID {id} not found.");
        }

        return Ok(tour);
    }

    [HttpPost]
    public IActionResult AddTour([FromBody] Tour newTour)
    {
        if (newTour == null)
        {
            return BadRequest("Invalid tour data.");
        }

        try
        {
            // Добавляем новый тур в контекст
            _context.Tours.Add(newTour);

            // Сохраняем изменения в базе данных
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetTours), new { id = newTour.Id }, newTour);
        }
        catch (Exception ex)
        {
            // Возвращаем ошибку при сбое
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTour(int id)
    {
        var tour = _context.Tours.FirstOrDefault(t => t.Id == id);
        if (tour == null)
        {
            return NotFound($"Tour with ID {id} not found.");
        }

        try
        {
            _context.Tours.Remove(tour);
            _context.SaveChanges();
            return Ok($"Tour with ID {id} has been deleted.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public IActionResult UpsertTour(int id, [FromBody] Tour tourData)
    {
        if (tourData == null || tourData.Id != id)
        {
            return BadRequest("Invalid tour data.");
        }

        var existingTour = _context.Tours.FirstOrDefault(t => t.Id == id);

        if (existingTour == null)
        {
            // Если тур не найден, создаем новый
            var newTour = new Tour
            {
                Id = id,
                LinkToCard = tourData.LinkToCard,
                TourName = tourData.TourName,
                ShortDescription = tourData.ShortDescription,
                AmountofPlaces = tourData.AmountofPlaces,
                Price = tourData.Price,
                Picture = tourData.Picture,
                DepartureCity = tourData.DepartureCity,
                StartDate = tourData.StartDate,
                FinishDate = tourData.FinishDate
            };

            try
            {
                _context.Tours.Add(newTour);
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetTours), new { id = newTour.Id }, newTour); // Возвращаем HTTP 201 с созданным туром
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        else
        {
            existingTour.Id = tourData.Id;
            // Если тур найден, обновляем его
            existingTour.LinkToCard = tourData.LinkToCard;
            existingTour.TourName = tourData.TourName;
            existingTour.ShortDescription = tourData.ShortDescription;
            existingTour.AmountofPlaces = tourData.AmountofPlaces;
            existingTour.Price = tourData.Price;
            existingTour.Picture = tourData.Picture;
            existingTour.DepartureCity = tourData.DepartureCity;
            existingTour.StartDate = tourData.StartDate;
            existingTour.FinishDate = tourData.FinishDate;

            try
            {
                _context.SaveChanges();
                return Ok(existingTour); // Возвращаем обновленный тур
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Tour>>> SearchTours([FromQuery] string query)
    {
        if (string.IsNullOrEmpty(query))
        {
            return BadRequest("Query parameter is required.");
        }

        var tours = await _context.Tours
            .Where(t => EF.Functions.Like(t.TourName, $"%{query}%"))
            .ToListAsync();

        return Ok(tours);

    }
}
