import http from 'k6/http';
import { sleep } from 'k6';

export let options = {
  vus: 50,
  duration: '30s',
};

export default function () {
  const url = 'http://localhost:5100/api/launches';
  const payload = JSON.stringify({ merchantId: '00000000-0000-0000-0000-000000000001', amount: 10.0, currency: 'BRL', occurredAt: new Date().toISOString() });
  const params = { headers: { 'Content-Type': 'application/json' } };
  http.post(url, payload, params);
  sleep(1);
}
