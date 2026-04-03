// Em wwwroot/js/NavBar/BarberShop/ShowBarberShops.js

document.addEventListener('DOMContentLoaded', function () {
    const searchForm = document.getElementById('search-form');
    const resultsContainer = document.getElementById('results-container');
    const loadingMessage = document.getElementById('loading-message');

    // Elementos de filtro
    const searchTermInput = document.getElementById('searchTerm');
    const cityInput = document.getElementById('city');
    const stateInput = document.getElementById('state');
    const barberInput = document.getElementById('barber');

    let searchTimeout; // Para debounce - evitar muitas requisições

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

            const averageRating = shop.averageRating > 0 ? shop.averageRating.toFixed(1) : 'N/A';
            const reviewCount = shop.reviewCount || 0;

            return `
            <div class="col-md-6 col-lg-4">
                <div class="card barber-card h-100">
                    <img src="${imageUrl}" class="card-img-top" alt="Foto de ${shop.name}">
                    <div class="card-body d-flex flex-column">
                        <h5 class="card-title fw-bold">${shop.name}</h5>
                        <p class="card-text text-white-50 mb-2"><i class="bi bi-geo-alt-fill"></i> ${shop.city || 'Cidade não informada'}, ${shop.state || 'UF'}</p>
                        
                        <div class="ratings mb-3">
                            <i class="bi bi-star-fill"></i>
                            <strong>${averageRating}</strong> 
                            <span class="text-white-50">(${reviewCount} ${reviewCount === 1 ? 'avaliação' : 'avaliações'})</span>
                        </div>
                        
                        <a href="/barbershop/details/${shop.barberShopId}" class="btn btn-gold mt-auto">Ver Detalhes</a>
                    </div>
                </div>
            </div>
        `;
        }).join('');

        resultsContainer.innerHTML = cardsHtml;
    }

    // Função para buscar com debounce (evita muitas requisições)
    function performSearch() {
        clearTimeout(searchTimeout);
        
        searchTimeout = setTimeout(() => {
            const params = {
                barberShopName: searchTermInput.value,
                city: cityInput.value,
                state: stateInput.value,
                barber: barberInput.value
            };

            fetchAndRenderBarberShops(params);
        }, 300); // Aguarda 300ms sem mudanças antes de buscar
    }

    // Listeners para busca em tempo real conforme digita
    searchTermInput.addEventListener('input', performSearch);
    cityInput.addEventListener('input', performSearch);
    stateInput.addEventListener('input', performSearch);
    barberInput.addEventListener('input', performSearch);

    // Listener do formulário para quando clica no botão "Filtrar"
    searchForm.addEventListener('submit', function (event) {
        event.preventDefault();
        performSearch();
    });

    // Lê parâmetros da URL no carregamento inicial (ex: ?barberShopName=...)
    const urlParams = new URLSearchParams(window.location.search);
    const initialParams = {
        barberShopName: urlParams.get('barberShopName') || '',
        city: urlParams.get('city') || '',
        state: urlParams.get('state') || '',
        barber: urlParams.get('barberName') || ''
    };
    
    // Preenche os campos com os parâmetros da URL
    if (initialParams.barberShopName) searchTermInput.value = initialParams.barberShopName;
    if (initialParams.city) cityInput.value = initialParams.city;
    if (initialParams.state) stateInput.value = initialParams.state;
    if (initialParams.barber) barberInput.value = initialParams.barber;
    
    // Se houver parâmetros na URL, usa-os; senão, carrega todas as barbearias
    fetchAndRenderBarberShops(initialParams);
});