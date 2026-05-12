using Grpc.Core;
using GRPCAirQualityService.Protos;

namespace GRPCAirQualityService.Services
{
    public class AirqualityService : AirQualityService.AirQualityServiceBase
    {
        /*public override Task<AirQualityService> AddSensorData(SensorDataRequest request, ServerCallContext context)
        {
            return Task.FromResult(new IngestResponse
            {
                Success = true
            });
        }*/
    }
}
