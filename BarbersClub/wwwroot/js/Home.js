document.addEventListener('DOMContentLoaded', function () {

    const resultsContainer = document.getElementById('results-container');
    const resultsTitle = document.getElementById('results-title');
    const searchForm = document.getElementById('search-form');
    const searchInput = document.getElementById('search-input');
    const serviceLinks = document.querySelectorAll('.service-filter-link');

    /**
     * Função genérica para buscar dados na API e exibir os resultados.
     * @param {string} url - A URL da API a ser chamada.
     * @param {string} searchType - O tipo de busca ('barbershop' ou 'service').
     * @param {string} title - O título a ser exibido na seção de resultados.
     */
    async function displayResults(url, searchType, title) {
        resultsTitle.textContent = title;
        resultsContainer.innerHTML = '<p class="text-center text-white-50">Buscando resultados...</p>';

        try {
            const response = await fetch(url);
            if (!response.ok) throw new Error('A resposta da rede não foi bem-sucedida.');

            const results = await response.json();
            resultsContainer.innerHTML = '';

            if (results.length === 0) {
                resultsContainer.innerHTML = '<p class="text-center text-white-50 fs-5">Nenhum resultado encontrado.</p>';
                return;
            }

            results.forEach(item => {
                const cardCreator = searchType === 'barbershop' ? createBarberShopCard : createServiceResultCard;
                resultsContainer.innerHTML += cardCreator(item);
            });
        } catch (error) {
            console.error('Erro ao buscar resultados:', error);
            resultsContainer.innerHTML = '<p class="text-center text-danger fs-5">Ocorreu um erro ao buscar os resultados.</p>';
        }
    }

    // Ação 1: Carregar os destaques quando a página abre
    displayResults('/api/barberShop/top-rated?count=6', 'barbershop', 'Barbearias em Destaque');


    // Ação 2: Ouvir o envio do formulário de busca
    searchForm.addEventListener('submit', function(event) {
        event.preventDefault(); // Impede que a página recarregue!
        const query = searchInput.value.trim();
        if (query) {
            const url = `/api/barberShop/search?barberShopName=${encodeURIComponent(query)}`;
            displayResults(url, 'barbershop', `Resultados para "${query}"`);
        }
    });

    // Ação 3: Ouvir cliques nos filtros de serviço
    serviceLinks.forEach(link => {
        link.addEventListener('click', function(event) {
            event.preventDefault(); // Impede que o link navegue!
            const serviceType = this.dataset.service;
            if (serviceType) {
                const url = `/api/services?serviceType=${encodeURIComponent(serviceType)}&sortBy=ratings_desc`;
                displayResults(url, 'service', `Resultados para o serviço "${serviceType}"`);
            }
        });
    });

});


// =================================================================================
// FUNÇÕES PARA CRIAR OS CARDS HTML
// =================================================================================

/**
 * Cria um card para uma barbearia (usado na busca por texto e nos destaques).
 * @param {object} shop - Objeto da barbearia.
 */
function createBarberShopCard(shop) {
    // Lógica para gerar as estrelas de avaliação (se houver dados)
    let ratingHtml = '';
    if (shop.averageRating && shop.reviewCount) {
        let starsHtml = '';
        const fullStars = Math.floor(shop.averageRating);
        const halfStar = shop.averageRating - fullStars >= 0.5;
        for(let i = 0; i < fullStars; i++) starsHtml += '<i class="bi bi-star-fill"></i>';
        if(halfStar) starsHtml += '<i class="bi bi-star-half"></i>';
        const emptyStars = 5 - fullStars - (halfStar ? 1 : 0);
        for(let i = 0; i < emptyStars; i++) starsHtml += '<i class="bi bi-star"></i>';

        ratingHtml = `<div class="rating mb-3">${starsHtml}<span class="ms-1">${shop.averageRating.toFixed(1)} (${shop.reviewCount} avaliações)</span></div>`;
    }

    return `
        <div class="col-md-6 col-lg-4 d-flex">
            <div class="barber-card w-100">
                <img src="${shop.profilePictureUrl || shop.profilePicUrl || '/images/placeholder-barbershop.jpg'}" class="card-img-top" alt="Foto da ${shop.name}">
                <div class="card-body">
                    <h5 class="card-title fw-bold">${shop.name}</h5>
                    <p class="mb-2"><i class="bi bi-geo-alt-fill me-1"></i> ${shop.city}, ${shop.state}</p>
                    ${ratingHtml}
                    <p class="text-white-50">${shop.description || 'Clique para ver os detalhes e agendar.'}</p>
                </div>
                <div class="card-footer p-3">
                    <a href="/barbershop/details/${shop.id || shop.barberShopId}" class="btn btn-outline-light w-100">Ver Perfil e Agendar</a>
                </div>
                </div>
            </div>
        </div>
    `;
}

/**
 * Cria um card para um resultado da busca por Serviço.
 * @param {object} service - Objeto ServiceViewResponse da API.
 */
function createServiceResultCard(service) {
    return `
        <div class="col-md-6 col-lg-4 d-flex">
            <div class="barber-card w-100">
                <img src="${service.serviceImageUrl || '/images/placeholder-barbershop.jpg'}" class="card-img-top" alt="Foto do serviço em ${service.barberShopName}">
                <div class="card-body">
                    <span class="badge bg-gold text-dark mb-2">${service.serviceType}</span>
                    <h5 class="card-title fw-bold">${service.barberShopName}</h5>
                    <p class="mb-2"><i class="bi bi-person-check-fill me-1"></i> Barbearia de: <strong>${service.barber}</strong></p>
                    <p class="text-white-50">Esta barbearia oferece o serviço que você procura. Clique para ver o perfil completo.</p>
                </div>
                <div class="card-footer p-3">
                    <a href="/Barbearia/${service.barberShopId}" class="btn btn-outline-light w-100">Ver Perfil da Barbearia</a>
                </div>
            </div>
        </div>
    `;
}