using System;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Weather.Domain.Models;

namespace Weather.Infrastructure.Services
{
    public class WeatherApiClient : IWeatherApiClient
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;
        public WeatherApiClient(HttpClient http, IConfiguration config)
        {
            _http = http;
            _apiKey = config["WeatherApi:ApiKey"] ?? throw new ArgumentNullException("WeatherApi:ApiKey");
        }

        public async Task<WeatherDto> GetWeatherForAsync(double latitude, double longitude)
        {
            var lat = latitude.ToString(CultureInfo.InvariantCulture);
            var lon = longitude.ToString(CultureInfo.InvariantCulture);
            
            var q = $"{lat},{lon}";
            var url = $"http://api.weatherapi.com/v1/forecast.json?key={_apiKey}&q={q}&days=3&aqi=no&alerts=no";
            var resp = await _http.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            using var stream = await resp.Content.ReadAsStreamAsync();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var raw = await JsonSerializer.DeserializeAsync<RawForecastResponse>(stream, options);

            var dto = new WeatherDto(
                new LocationDto(raw.Location.Name, raw.Location.Region, raw.Location.Country, raw.Location.Lat, raw.Location.Lon, raw.Location.TzId, raw.Location.Localtime),
                new CurrentDto(raw.Current.TempC, raw.Current.IsDay, new ConditionDto(raw.Current.Condition.Text, raw.Current.Condition.Icon), raw.Current.Humidity, raw.Current.WindKph),
                raw.Forecast.Forecastday != null
                    ? raw.Forecast.Forecastday.ConvertAll(fd => new ForecastDayDto(fd.Date, new DayDto(fd.Day.MaxTempC, fd.Day.MinTempC, fd.Day.AvgTempC, new ConditionDto(fd.Day.Condition.Text, fd.Day.Condition.Icon)), fd.Hour.ConvertAll(h => new HourDto(h.Time, h.TempC, new ConditionDto(h.Condition.Text, h.Condition.Icon)))))
                    : Array.Empty<ForecastDayDto>()
            );

            return dto;
        }

        private class RawForecastResponse
        {
            public RawLocation Location { get; set; }
            public RawCurrent Current { get; set; }
            public RawForecast Forecast { get; set; }
        }
        private class RawLocation
        {
            public string Name { get; set; }
            public string Region { get; set; }
            public string Country { get; set; }
            public double Lat { get; set; }
            public double Lon { get; set; }
            public string TzId { get; set; }
            public string Localtime { get; set; }
        }
        private class RawCurrent
        {
            [System.Text.Json.Serialization.JsonPropertyName("temp_c")] public double TempC { get; set; }
            [System.Text.Json.Serialization.JsonPropertyName("is_day")] public int IsDay { get; set; }
            public RawCondition Condition { get; set; }
            public int Humidity { get; set; }
            [System.Text.Json.Serialization.JsonPropertyName("wind_kph")] public double WindKph { get; set; }
        }
        private class RawForecast
        {
            public System.Collections.Generic.List<RawForecastDay> Forecastday { get; set; }
        }
        private class RawForecastDay
        {
            public string Date { get; set; }
            public RawDay Day { get; set; }
            public System.Collections.Generic.List<RawHour> Hour { get; set; }
        }
        private class RawDay
        {
            [System.Text.Json.Serialization.JsonPropertyName("maxtemp_c")] public double MaxTempC { get; set; }
            [System.Text.Json.Serialization.JsonPropertyName("mintemp_c")] public double MinTempC { get; set; }
            [System.Text.Json.Serialization.JsonPropertyName("avgtemp_c")] public double AvgTempC { get; set; }
            public RawCondition Condition { get; set; }
        }
        private class RawHour
        {
            public string Time { get; set; }
            [System.Text.Json.Serialization.JsonPropertyName("temp_c")] public double TempC { get; set; }
            public RawCondition Condition { get; set; }
        }
        private class RawCondition
        {
            public string Text { get; set; }
            public string Icon { get; set; }
        }
    }
}
