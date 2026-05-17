import grpc from 'k6/net/grpc';
import { check, sleep } from 'k6';

const client = new grpc.Client();
client.load(['.'], 'airquality.proto');

export const options = {
  stages: [
    { duration: '20s', target: 10 },
    { duration: '20s', target: 100 },
    { duration: '20s', target: 500 },
  ],
};

export default function () {
  client.connect('127.0.0.1:5000', { plaintext: true });

  const start = new Date(Date.now() - 24 * 60 * 60 * 1000).toISOString();
  const end = new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString();

  // U .proto fajlu za TimeRangeRequest parametri su start_time i end_time
  const data = { 
    device_id: `sensor-${__VU}`,
    start_time: start,
    end_time: end
  }; 

  const response = client.invoke('airquality.AirQualityService/GetAggregation', data);

  check(response, {
    'status je OK': (r) => r && r.status === grpc.StatusOK,
  });

  client.close();
  sleep(1);
}