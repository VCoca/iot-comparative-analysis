using Grpc.Core;
using GRPCAirQualityService.Protos;
using Npgsql;
using Dapper;

namespace GRPCAirQualityService.Services
{
    public class AirqualityService : AirQualityService.AirQualityServiceBase
    {
        private readonly string _connectionString;

        public AirqualityService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public override async Task<IngestResponse> AddSensorData(SensorDataRequest request, ServerCallContext context)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var sql = @"INSERT INTO sensor_data (device_id, recorded_at, co_gt, temperature, relative_humidity) 
                        VALUES (@DeviceId, @RecordedAt, @CoGt, @Temperature, @RelativeHumidity)";
            var result = await connection.ExecuteAsync(sql, new
            {
                DeviceId = request.DeviceId,
                RecordedAt = request.RecordedAt.ToDateTime(),
                CoGt = request.CoGt,
                Temperature = request.Temperature,
                RelativeHumidity = request.RelativeHumidity
            });
            return new IngestResponse
            {
                Success = result > 0
            };
        }
        public override async Task<SensorDataResponse> GetLatestData(DeviceRequest request, ServerCallContext context)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var sql = @"SELECT device_id, recorded_at, co_gt, temperature, relative_humidity 
                        FROM sensor_data 
                        WHERE device_id = @DeviceId 
                        ORDER BY recorded_at DESC 
                        LIMIT 1";
            var result = await connection.QueryFirstOrDefaultAsync(sql, new { DeviceId = request.DeviceId });
            if (result == null)
            {
                return new SensorDataResponse();
            }

            return new SensorDataResponse
            {
                DeviceId = result.device_id,

                RecordedAt = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.SpecifyKind(result.recorded_at, DateTimeKind.Utc)),

                CoGt = result.co_gt ?? 0,
                Temperature = result.temperature ?? 0,
                RelativeHumidity = result.relative_humidity ?? 0
            };
        }

        public override async Task<AggregationResponse> GetAggregation(TimeRangeRequest request, ServerCallContext context)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var sql = @"SELECT 
                            AVG(temperature) AS avg_temperature, 
                            MAX(co_gt) AS max_co, 
                            MIN(relative_humidity) AS min_humidity 
                        FROM sensor_data 
                        WHERE device_id = @DeviceId 
                        AND recorded_at BETWEEN @StartTime AND @EndTime";
            var result = await connection.QueryFirstOrDefaultAsync(sql, new
            {
                DeviceId = request.DeviceId,
                StartTime = request.StartTime.ToDateTime(),
                EndTime = request.EndTime.ToDateTime()
            });
            return new AggregationResponse
            {
                AvgTemperature = result.avg_temperature ?? 0,
                MaxCo = result.max_co ?? 0,
                MinHumidity = result.min_humidity ?? 0
            };
        }
    }
}
