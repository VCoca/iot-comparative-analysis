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

  const randomTemp = Math.random() * 40; 
  const randomHum = Math.random() * 100;

  const data = {
    deviceId: `sensor-${__VU}`,
    recordedAt: new Date().toISOString(),
    co_gt: 1.5, 
    nmhc_gt: 250,
    c6h6_gt: 15,
    nox_gt: 200,
    no2_gt: 120,
    temperature: randomTemp,
    relativeHumidity: randomHum
  };

  // Pretpostavka je da se metoda za upis u .proto fajlu zove AddSensorData
  const response = client.invoke('airquality.AirQualityService/AddSensorData', data);

  check(response, {
    'status je OK': (r) => r && r.status === grpc.StatusOK,
  });

  client.close();
  sleep(1);
}