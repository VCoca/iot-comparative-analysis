namespace RESTAirQualityService.Models
{
    public class SensorDataRequest
    {
        public string DeviceId { get; set; }
        public DateTime RecordedAt { get; set; }
        public double CoGt { get; set; }
        public double Temperature { get; set; }
        public double RelativeHumidity { get; set; }
    }

    public class SensorDataResponse : SensorDataRequest
    {
        public long Id { get; set; }
    }

    public class AggregationResponse
    {
        public double AvgTemperature { get; set; }
        public double MaxCo { get; set; }
        public double MinHumidity { get; set; }
    }
}
