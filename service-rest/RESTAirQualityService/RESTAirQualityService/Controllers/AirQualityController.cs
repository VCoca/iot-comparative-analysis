using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Dapper;
using RESTAirQualityService.Models;

namespace RESTAirQualityService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AirQualityController : ControllerBase
    {
        private readonly string _connectionString;

        public AirQualityController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost]
        public async Task<IActionResult> AddSensorData([FromBody] SensorDataRequest request)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var sql = @"INSERT INTO sensor_data (device_id, recorded_at, co_gt, nmhc_gt, c6h6_gt, nox_gt, no2_gt, temperature, relative_humidity) 
                        VALUES (@DeviceId, @RecordedAt, @CoGt, @NmhcGt, @C6h6Gt, @NoxGt, @No2Gt, @Temperature, @RelativeHumidity)";

            var result = await connection.ExecuteAsync(sql, request);

            if (result > 0) return Ok(new { success = true });
            return BadRequest(new { success = false, message = "Failed to insert" });
        }

        [HttpGet("latest/{deviceId}")]
        public async Task<IActionResult> GetLatestData(string deviceId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var sql = @"SELECT id, device_id AS DeviceId, recorded_at AS RecordedAt, 
                               co_gt AS CoGt, nmhc_gt AS NmhcGt, c6h6_gt AS C6h6Gt, nox_gt AS NoxGt, no2_gt AS No2Gt, temperature AS Temperature, relative_humidity AS RelativeHumidity 
                        FROM sensor_data 
                        WHERE device_id = @DeviceId 
                        ORDER BY recorded_at DESC 
                        LIMIT 1";

            var data = await connection.QueryFirstOrDefaultAsync<SensorDataResponse>(sql, new { DeviceId = deviceId });

            if (data == null) return NotFound(new { message = "Nema podataka za ovaj uređaj" });
            return Ok(data);
        }

        [HttpGet("aggregate/{deviceId}")]
        public async Task<IActionResult> GetAggregation(string deviceId, [FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var sql = @"SELECT 
                            AVG(temperature) AS AvgTemperature, 
                            MAX(co_gt) AS MaxCo, 
                            MAX(nmhc_gt) AS MaxNmhc, 
                            MAX(c6h6_gt) AS MaxC6h6, 
                            MAX(nox_gt) AS MaxNox,
                            MAX(no2_gt) AS MaxNo2,
                            MIN(relative_humidity) AS MinHumidity 
                        FROM sensor_data 
                        WHERE device_id = @DeviceId AND recorded_at BETWEEN @Start AND @End";

            var result = await connection.QueryFirstOrDefaultAsync<AggregationResponse>(sql, new { DeviceId = deviceId, Start = start, End = end });

            return Ok(result ?? new AggregationResponse());
        }
    }
}
