const CACHE_NAME = 'car-assistant-cache-v2';

const OFFLINE_URLS = [
	'/',
	'/Auth/Index',
	'/Home/Index',
	'/css/main.css',
	'/js/index.js',
	'/js/dairy.js',
	'/js/consumation.js',
	'/js/statistic.js'
];

self.addEventListener('install', event => {
	event.waitUntil(
		caches.open(CACHE_NAME)
			.then(cache => cache.addAll(OFFLINE_URLS))
			.then(self.skipWaiting())
	);
});

self.addEventListener('activate', event => {
	event.waitUntil(
		caches.keys().then(keys =>
			Promise.all(
				keys
					.filter(key => key !== CACHE_NAME)
					.map(key => caches.delete(key))
			)
		).then(() => self.clients.claim())
	);
});

self.addEventListener('fetch', event => {
	const { request } = event;

	// Only GET requests
	if (request.method !== 'GET') {
		return;
	}

	// Для HTML/навигационных запросов используем стратегию network-first,
	// чтобы всегда видеть актуальные данные (машина, расходы и т.п.).
	if (request.mode === 'navigate' || (request.headers.get('accept') || '').includes('text/html')) {
		event.respondWith(
			fetch(request)
				.then(networkResponse => {
					const copy = networkResponse.clone();
					caches.open(CACHE_NAME).then(cache => {
						cache.put(request, copy);
					});
					return networkResponse;
				})
				.catch(() => caches.match(request) || caches.match('/Home/Index') || caches.match('/Auth/Index'))
		);
		return;
	}

	// Для статики (CSS/JS) оставляем cache-first
	event.respondWith(
		caches.match(request)
			.then(cachedResponse => {
				if (cachedResponse) {
					return cachedResponse;
				}

				return fetch(request)
					.then(networkResponse => {
						const copy = networkResponse.clone();
						caches.open(CACHE_NAME).then(cache => {
							cache.put(request, copy);
						});
						return networkResponse;
					});
			})
	);
});

