import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  stages: [
    { duration: '20s', target: 10 },
    { duration: '20s', target: 100 },
    { duration: '20s', target: 500 },
  ],
};

export default function () {
  const url = 'http://localhost:4000/';
  const randomTemp = Math.random() * 40; 
  const randomHum = Math.random() * 100;

  const payload = JSON.stringify({
    query: `
      mutation {
        addSensorData(
          device_id: "sensor-${__VU}", 
          recorded_at: "${new Date().toISOString()}", 
          co_gt: 1.5, 
          nmhc_gt: 250,
          c6h6_gt: 15,
          nox_gt: 200,
          no2_gt: 120,
          temperature: ${randomTemp}, 
          relative_humidity: ${randomHum}
        )
      }
    `
  });

  const params = {
    headers: {
      'Content-Type': 'application/json',
    },
  };

  const res = http.post(url, payload, params);

  check(res, {
    'status je 200': (r) => r.status === 200,
  });

  sleep(1);
}