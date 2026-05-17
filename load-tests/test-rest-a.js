import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  // Ovo ispunjava uslov profesora za 10, 100 i 500 korisnika!
  stages: [
    { duration: '20s', target: 10 },  // Prvih 20 sekundi diže na 10 korisnika
    { duration: '20s', target: 100 }, // Sledećih 20 sekundi diže na 100 korisnika
    { duration: '20s', target: 500 }, // Poslednjih 20 sekundi zakucava na 500 korisnika
  ],
};

export default function () {
  const url = 'http://localhost:5001/api/AirQuality';
  
  // Generišemo neki random broj za senzor da ne upisujemo uvek isto
  const randomTemp = Math.random() * 40; 
  const randomHum = Math.random() * 100;

  const payload = JSON.stringify({
    deviceId: `sensor-${__VU}`, // __VU je ID virtuelnog korisnika (od 1 do 500)
    recordedAt: new Date().toISOString(),
    coGt: 1.5,
    nmhcGT: 250,
    c6h6GT: 15,
    noxGT: 200,
    no2GT: 120,
    temperature: randomTemp,
    relativeHumidity: randomHum
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

  sleep(1); // Senzor šalje podatak svake sekunde
}