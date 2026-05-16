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
            var sql = @"INSERT INTO sensor_data (device_id, recorded_at, co_gt, nmhc_gt, c6h6_gt, nox_gt, no2_gt, temperature, relative_humidity) 
                        VALUES (@DeviceId, @RecordedAt, @CoGt, @NmhcGt, @C6h6Gt, @NoxGt, @NTemperature, @RelativeHumidity)";
            var result = await connection.ExecuteAsync(sql, new
            {
                DeviceId = request.DeviceId,
                RecordedAt = request.RecordedAt.ToDateTime(),
                CoGt = request.CoGt,
                NmhcGt = request.NmhcGt,
                C6H6Gt = request.C6H6Gt,
                NoxGt = request.NoxGt,
                No2Gt = request.No2Gt,
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
            var sql = @"SELECT device_id, recorded_at, co_gt, nmhc_gt, c6h6_gt, nox_gt, no2_gt, temperature, relative_humidity 
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
                NmhcGt = result.nmhc_gt ?? 0,
                C6H6Gt = result.c6h6_gt ?? 0,
                NoxGt = result.nox_gt ?? 0,
                No2Gt = result.no2_gt ?? 0,
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
                            MAX(nmhc_gt) AS max_nmhc,
                            MAX(c6h6_gt) AS max_c6h6,
                            MAX(nox_gt) AS max_nox,
                            MAX(no2_gt) AS max_no2,
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
                MaxNmhc = result.max_nmhc ?? 0,
                MaxC6H6 = result.max_c6h6 ?? 0,
                MaxNox = result.max_nox ?? 0,
                MaxNo2 = result.max_no2 ?? 0,
                MinHumidity = result.min_humidity ?? 0
            };
        }
    }
}
