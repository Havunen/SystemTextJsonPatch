using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SystemTextJsonPatch.ApiTest.Controllers
{
	[Route("api/[controller]")]
	public class WeatherController : ControllerBase
	{
		private readonly WeatherForecast[] _weatherForecasts = new WeatherForecast[]
		{
			new WeatherForecast { Date = DateTime.Now, TemperatureC = 25, Summary = "Hot" },
			new WeatherForecast { Date = DateTime.Now.AddDays(1), TemperatureC = 20, Summary = "Warm" },
			new WeatherForecast { Date = DateTime.Now.AddDays(2), TemperatureC = 15, Summary = "Cool" },
			new WeatherForecast { Date = DateTime.Now.AddDays(3), TemperatureC = 10, Summary = "Cold" },
			new WeatherForecast { Date = DateTime.Now.AddDays(4), TemperatureC = 5, Summary = "Freezing" }
		};

		[HttpGet]
		public IActionResult GetWeather()
		{
			return Ok(_weatherForecasts);
		}

		[HttpPost]
		public ActionResult<WeatherForecast> PostWeather([FromBody, Required] WeatherForecast? weatherForecast)
		{
			if (weatherForecast == null || ModelState.IsValid == false)
			{
				return ValidationProblem(this.ModelState);
			}

			return CreatedAtAction(nameof(GetWeather), weatherForecast);
		}

		[HttpPatch("{id}")]
		public IActionResult PatchWeather(int id, [FromBody, Required] JsonPatchDocument<WeatherForecast>? patchDoc)
		{
			var weatherForecast = _weatherForecasts[id];
			if (weatherForecast == null)
			{
				return NotFound();
			}

			patchDoc.ApplyTo(weatherForecast);
			TryValidateModel(weatherForecast);
			if (ModelState.IsValid == false)
			{
				return ValidationProblem(this.ModelState);
			}

			return Ok();
		}
	}

	public class WeatherForecast
	{
		public DateTime Date { get; set; }
		[Required]
		public int? TemperatureC { get; set; }
		public string Summary { get; set; }
	}
}
