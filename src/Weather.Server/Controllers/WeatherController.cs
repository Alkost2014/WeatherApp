using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Weather.Application.Queries;

namespace Weather.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _config;
        private double _latitude;
        private double _longitude;

        //public WeatherController(IMediator mediator) => _mediator = mediator;
        public WeatherController(IMediator mediator, IConfiguration config)
        {
            _mediator = mediator;
            _config = config;
        } 

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            //// Москва (широта)
            //var lat = 55.7558;
            //// Москва (долгота)
            //var lon = 37.6173;

            try
            {
                _latitude = _config.GetValue<double>("WeatherApi:latitude");
                _longitude = _config.GetValue<double>("WeatherApi:longitude");

                var result = await _mediator.Send(new GetWeatherQuery(_latitude, _longitude));
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}
