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
  // Gađamo sensor-1 koji sigurno postoji u bazi nakon Scenarija A
  const res = http.get('http://localhost:5001/api/AirQuality/latest/sensor-1');

  check(res, {
    'status je 200': (r) => r.status === 200,
  });

  sleep(1);
}