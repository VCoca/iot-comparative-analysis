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
  const payload = JSON.stringify({
    query: `
      query {
        latestData(device_id: "sensor-1") {
          temperature
          relative_humidity
        }
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