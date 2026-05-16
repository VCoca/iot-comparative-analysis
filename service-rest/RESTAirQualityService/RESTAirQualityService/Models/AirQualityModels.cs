namespace RESTAirQualityService.Models
{
    public class SensorDataRequest
    {
        public string DeviceId { get; set; }
        public DateTime RecordedAt { get; set; }
        public double CoGt { get; set; }
        public double NmhcGt { get; set; }
        public double C6h6Gt { get; set; }
        public double NoxGt { get; set; }
        public double No2Gt { get; set; }
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
        public double MaxNmhc { get; set; }
        public double MaxC6h6 { get; set; }
        public double MaxNox { get; set; }
        public double MaxNo2 { get; set; }
        public double MinHumidity { get; set; }
    }
}
