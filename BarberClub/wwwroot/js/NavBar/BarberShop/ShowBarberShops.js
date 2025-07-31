// Em wwwroot/js/NavBar/BarberShops.js

document.addEventListener('DOMContentLoaded', function () {
    const searchForm = document.getElementById('search-form');
    const resultsContainer = document.getElementById('results-container');
    const loadingMessage = document.getElementById('loading-message');

    async function fetchAndRenderBarberShops(params = {}) {
        loadingMessage.style.display = 'block';
        resultsContainer.innerHTML = '';

        try {
            const urlParams = new URLSearchParams();

            if (params.barberShopName) urlParams.append('barberShopName', params.barberShopName);
            if (params.city) urlParams.append('city', params.city);
            if (params.state) urlParams.append('state', params.state);
            if (params.barber) urlParams.append('barberName', params.barber);

            const hasSearchParams = Array.from(urlParams.values()).some(v => v);

            const endpoint = hasSearchParams
                ? `/barberShopApi/search?${urlParams}`
                : '/barberShopApi';

            const response = await fetch(endpoint);

            if (!response.ok) {
                throw new Error(`Erro na rede: ${response.statusText}`);
            }

            const responseData = await response.json(); // Pega o objeto completo
            renderResults(responseData.$values);

        } catch (error) {
            console.error('Falha ao buscar barbearias:', error);
            resultsContainer.innerHTML = '<div class="col-12 text-center text-white-50"><p class="fs-4">Ocorreu um erro ao carregar os dados. Tente novamente mais tarde.</p></div>';
        } finally {
            loadingMessage.style.display = 'none';
        }
    }

    function renderResults(shops) {
        
        if (!shops || shops.length === 0) {
            resultsContainer.innerHTML = '<div class="col-12 text-center text-white-50"><p class="fs-4">Nenhuma barbearia encontrada.</p></div>';
            return;
        }

        const cardsHtml = shops.map(shop => `
            <div class="col-md-6 col-lg-4">
                <div class="card barber-card h-100">
                    <img src="${shop.imageUrl || 'https://via.placeholder.com/400x250'}" class="card-img-top" alt="Foto de ${shop.name}">
                    <div class="card-body d-flex flex-column">
                        <h5 class="card-title fw-bold">${shop.name}</h5>
                        <p class="card-text text-white-50"><i class="bi bi-geo-alt-fill"></i> ${shop.city}, ${shop.state}</p>
                        <div class="ratings mb-3">
                            <i class="bi bi-star-fill"></i>
                            <span>${shop.rating || 'N/A'}</span>
                        </div>
                        <a href="/barbershop/details/${shop.barberShopId}" class="btn btn-gold mt-auto">Ver Detalhes</a>
                    </div>
                </div>
            </div>
        `).join('');

        resultsContainer.innerHTML = cardsHtml;
    }

    searchForm.addEventListener('submit', function (event) {
        event.preventDefault();

        const formData = new FormData(searchForm);

        const params = {
            barberShopName: formData.get('searchTerm'),
            city: formData.get('city'),
            state: formData.get('state'), 
            barber: formData.get('barber')
        };

        fetchAndRenderBarberShops(params);
    });

    fetchAndRenderBarberShops();
});