using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Weather.Domain.Models
{
    public record WeatherDto(
        LocationDto Location,
        CurrentDto Current,
        IEnumerable<ForecastDayDto> ForecastDays
    );

    public record LocationDto(
        string Name,
        string Region,
        string Country,
        [property: JsonPropertyName("lat")] double Lat,
        [property: JsonPropertyName("lon")] double Lon,
        string TzId,
        [property: JsonPropertyName("localtime")] string LocalTime
    );

    public record CurrentDto(
        [property: JsonPropertyName("temp_c")] double TempC,
        [property: JsonPropertyName("is_day")] int IsDay,
        ConditionDto Condition,
        int Humidity,
        [property: JsonPropertyName("wind_kph")] double WindKph
    );

    public record ForecastDayDto(
        [property: JsonPropertyName("date")] string Date,
        DayDto Day,
        IEnumerable<HourDto> Hour
    );

    public record DayDto(
        [property: JsonPropertyName("maxtemp_c")] double MaxTempC,
        [property: JsonPropertyName("mintemp_c")] double MinTempC,
        [property: JsonPropertyName("avgtemp_c")] double AvgTempC,
        ConditionDto Condition
    );

    public record HourDto(
        [property: JsonPropertyName("time")] string Time,
        [property: JsonPropertyName("temp_c")] double TempC,
        ConditionDto Condition
    );

    public record ConditionDto(
        string Text,
        string Icon
    );
}
