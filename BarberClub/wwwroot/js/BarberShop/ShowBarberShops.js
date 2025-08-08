// Em wwwroot/js/NavBar/BarberShop/ShowBarberShops.js

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
                ? `/api/barberShop/search?${urlParams}`
                : '/api/barberShop';

            const response = await fetch(endpoint);

            if (!response.ok) {
                throw new Error(`Erro na rede: ${response.statusText}`);
            }

            const responseData = await response.json();

            // --- CORREÇÃO APLICADA AQUI ---
            // Agora o 'responseData' já é o array de barbearias.
            renderResults(responseData);

        } catch (error) {
            console.error('Falha ao buscar barbearias:', error);
            resultsContainer.innerHTML = '<div class="col-12 text-center text-white-50"><p class="fs-4">Ocorreu um erro ao carregar os dados. Tente novamente mais tarde.</p></div>';
        } finally {
            loadingMessage.style.display = 'none';
        }
    }

    function renderResults(shops) {
        if (!shops || !Array.isArray(shops) || shops.length === 0) {
            resultsContainer.innerHTML = '<div class="col-12 text-center text-white-50"><p class="fs-4">Nenhuma barbearia encontrada.</p></div>';
            return;
        }

        const cardsHtml = shops.map(shop => {
            const imageUrl = shop.profilePicUrl || '/images/default-barbershop.png';
            
            // Calcula a média das avaliações
            const averageRating = (shop.ratings && shop.ratings.length > 0)
                ? (shop.ratings.reduce((acc, r) => acc + r.ratingValue, 0) / shop.ratings.length).toFixed(1)
                : 'N/A';

            return `
                <div class="col-md-6 col-lg-4">
                    <div class="card barber-card h-100">
                        <img src="${imageUrl}" class="card-img-top" alt="Foto de ${shop.name}">
                        <div class="card-body d-flex flex-column">
                            <h5 class="card-title fw-bold">${shop.name}</h5>
                            <p class="card-text text-white-50"><i class="bi bi-geo-alt-fill"></i> ${shop.city || 'Cidade não informada'}, ${shop.state || 'UF'}</p>
                            <div class="ratings mb-3">
                                <i class="bi bi-star-fill"></i>
                                <span>${averageRating}</span>
                            </div>
                            <a href="/barbershop/details/${shop.barberShopId}" class="btn btn-gold mt-auto">Ver Detalhes</a>
                        </div>
                    </div>
                </div>
            `;
        }).join('');

        resultsContainer.innerHTML = cardsHtml;
    }

    searchForm.addEventListener('submit', function (event) {
        event.preventDefault();

        const params = {
            barberShopName: document.getElementById('searchTerm').value,
            city: document.getElementById('city').value,
            state: '',
            barber: document.getElementById('barber').value
        };

        fetchAndRenderBarberShops(params);
    });

    // Carrega a lista inicial de barbearias
    fetchAndRenderBarberShops();
});