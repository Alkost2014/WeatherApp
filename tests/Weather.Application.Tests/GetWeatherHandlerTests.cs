using System.Threading;
using System.Threading.Tasks;
using Moq;
using Weather.Application.Queries;
using Weather.Domain.Models;
using Weather.Infrastructure.Services;
using Xunit;

namespace Weather.Application.Tests
{
    public class GetWeatherHandlerTests
    {
        [Fact]
        public async Task Handler_ReturnsDto_FromClient()
        {
            var mockClient = new Mock<IWeatherApiClient>();
            var sample = new WeatherDto(
                new LocationDto("Moscow","Moscow","Russia",55.7558,37.6173,"Europe/Moscow","2026-08-31 12:00"),
                new CurrentDto(20,1,new ConditionDto("Sunny",""),50,10),
                new[]{ new ForecastDayDto("2026-08-31", new DayDto(22,12,18,new ConditionDto("Sunny","")), new[]{ new HourDto("2026-08-31 13:00",20,new ConditionDto("Sunny","")) }) }
            );
            mockClient.Setup(c => c.GetWeatherForAsync(It.IsAny<double>(), It.IsAny<double>())).ReturnsAsync(sample);

            var handler = new GetWeatherHandler(mockClient.Object);
            var result = await handler.Handle(new GetWeatherQuery(55.7558,37.6173), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal("Moscow", result.Location.Name);
            Assert.Equal(20, result.Current.TempC);
        }
    }
}
