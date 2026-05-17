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
  const start = new Date(Date.now() - 24 * 60 * 60 * 1000).toISOString();
  const end = new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString();

  // Tražimo sve agregacije koje je server definisao
  const payload = JSON.stringify({
    query: `
      query {
        aggregateData(device_id: "sensor-${__VU}", start: "${start}", end: "${end}") {
          avg_temperature
          max_co
          max_nmhc
          max_c6h6
          max_nox
          max_no2
          min_humidity
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