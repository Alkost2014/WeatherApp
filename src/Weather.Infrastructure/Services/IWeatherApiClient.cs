using System.Threading.Tasks;
using Weather.Domain.Models;

namespace Weather.Infrastructure.Services
{
    public interface IWeatherApiClient
    {
        Task<WeatherDto> GetWeatherForAsync(double lat, double lon);
    }
}
