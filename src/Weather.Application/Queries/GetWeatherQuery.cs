using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Weather.Domain.Models;
using Weather.Infrastructure.Services;

namespace Weather.Application.Queries
{
    public record GetWeatherQuery(double Lat, double Lon) : IRequest<WeatherDto>;

    public class GetWeatherHandler : IRequestHandler<GetWeatherQuery, WeatherDto>
    {
        private readonly IWeatherApiClient _client;
        public GetWeatherHandler(IWeatherApiClient client) => _client = client;

        public async Task<WeatherDto> Handle(GetWeatherQuery request, CancellationToken cancellationToken)
        {
            var dto = await _client.GetWeatherForAsync(request.Lat, request.Lon);
            return dto;
        }
    }
}
