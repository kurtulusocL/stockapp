
self.addEventListener('install', (event) => {
    event.waitUntil(
        caches.open('v1').then((cache) => {
            return cache.addAll([
                '/',
                '/css/style.css',
                '/js/download.js',
                '/img/statics/logo192.png',
                '/img/statics/logo168.png',
                '/img/statics/logo512.png',
                '/icon.png'
            ]);            
        })
    );
});

self.addEventListener('fetch', (event) => {
    event.respondWith(
        caches.match(event.request).then((response) => {
            return response || fetch(event.request);
        })
    );
});