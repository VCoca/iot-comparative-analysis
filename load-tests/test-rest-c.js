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
  // Postavljamo širok vremenski opseg (od juče do sutra) da bismo uhvatili podatke
  const start = new Date(Date.now() - 24 * 60 * 60 * 1000).toISOString();
  const end = new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString();
  
  // REST prima datume preko Query parametara u URL-u
  const url = `http://localhost:5001/api/AirQuality/aggregate/sensor-${__VU}?start=${encodeURIComponent(start)}&end=${encodeURIComponent(end)}`;

  const res = http.get(url);

  check(res, {
    'status je 200': (r) => r.status === 200,
  });

  sleep(1);
}