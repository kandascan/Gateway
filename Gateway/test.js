import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    vus: 10, // Liczba równoczesnych użytkowników (Virtual Users)
    duration: '30s', // Czas trwania testu
};

export default function () {
    // 1. Wysyłamy żądanie do enqueowania zadania
    let enqueueResponse = http.get('http://localhost:5227/setProductRequestToQueue');

    check(enqueueResponse, {
        'enqueue response is 200': (res) => res.status === 200,
    });

    let requestId = JSON.parse(enqueueResponse.body).requestId;

    sleep(1); // Czekamy chwilę na przetworzenie zadania

    // 2. Pobieramy wynik z kolejki
    let resultResponse = http.get(`http://localhost:5227/getResponsFromCache/${requestId}?type=products`);

    check(resultResponse, {
        'result response is 200': (res) => res.status === 200 || res.status === 404,
    });

    sleep(1); // Krótka przerwa przed następnym żądaniem
}


//k6 run test.js



