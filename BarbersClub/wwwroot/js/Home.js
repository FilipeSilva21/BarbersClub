document.addEventListener('DOMContentLoaded', function () {

    const resultsContainer = document.getElementById('results-container');
    const resultsTitle = document.getElementById('results-title');
    const searchForm = document.getElementById('search-form');
    const searchInput = document.getElementById('search-input');

    async function displayResults(url, title) {
        resultsTitle.textContent = title;
        resultsContainer.innerHTML = '<p class="text-center text-white-50">Buscando resultados...</p>';

        try {
            const response = await fetch(url);
            if (!response.ok) throw new Error('A resposta da rede não foi bem-sucedida.');

            const results = await response.json();
            resultsContainer.innerHTML = '';

            if (results.length === 0) {
                resultsContainer.innerHTML = '<p class="text-center text-white-50 fs-5">Nenhum resultado encontrado.</p>';
            } else {
                results.forEach(item => {
                    resultsContainer.innerHTML += createBarberShopCard(item);
                });
            }
        } catch (error) {
            console.error('Erro ao buscar resultados:', error);
            resultsContainer.innerHTML = '<p class="text-center text-danger fs-5">Ocorreu um erro ao buscar os resultados.</p>';
        }
    }

    displayResults('/api/barberShop/top-rated?count=6', 'Barbearias em Destaque');


    searchForm.addEventListener('submit', function(event) {
        event.preventDefault();
        const query = searchInput.value.trim();
        if (query) {
            window.location.href = `/barbershops?barberShopName=${encodeURIComponent(query)}`;
        }
    });
});

// =================================================================================
// FUNÇÕES PARA CRIAR OS CARDS HTML
// =================================================================================

/**
 * Cria um card para uma barbearia.
 * @param {object} shop - Objeto da barbearia.
 */
function createBarberShopCard(shop) {
    const rating = shop.averageRating ? shop.averageRating.toFixed(1) : 'N/A';
    const reviewCount = shop.reviewCount || 0;
    const profilePic = shop.profilePictureUrl || shop.profilePicUrl || '/images/default-barbershop.png';
    const shopId = shop.id || shop.barberShopId;
    const shopName = shop.name || shop.barberShopName;

    let ratingHtml = `<div class="rating mb-3"><span>Sem avaliações</span></div>`;
    if (reviewCount > 0) {
        ratingHtml = `<div class="rating mb-3"><i class="bi bi-star-fill"></i><span class="ms-1">${rating} (${reviewCount} avaliações)</span></div>`;
    }

    return `
        <div class="col-md-6 col-lg-4 d-flex">
            <div class="barber-card w-100">
                <img src="${profilePic}" class="card-img-top" alt="Foto da ${shopName}">
                <div class="card-body">
                    <h5 class="card-title fw-bold">${shopName}</h5>
                    <p class="mb-2"><i class="bi bi-geo-alt-fill me-1"></i> ${shop.city}, ${shop.state}</p>
                    ${ratingHtml}
                    <p class="text-white-50">${shop.description || 'Clique para ver os detalhes e agendar.'}</p>
                </div>
                <div class="card-footer p-3">
                    <a href="/barbershop/details/${shopId}" class="btn btn-outline-light w-100">Ver Perfil e Agendar</a>
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
                <img src="${service.serviceImageUrl || '/images/default-service.png'}" class="card-img-top" alt="Foto do serviço em ${service.barberShopName}">
                <div class="card-body">
                    <span class="badge bg-gold text-dark mb-2">${service.serviceType}</span>
                    <h5 class="card-title fw-bold">${service.barberShopName}</h5>
                    <p class="mb-2"><i class="bi bi-person-check-fill me-1"></i> Barbearia de: <strong>${service.barber}</strong></p>
                    <p class="text-white-50">Esta barbearia oferece o serviço que você procura. Clique para ver o perfil completo.</p>
                </div>
                <div class="card-footer p-3">
                    <!-- CORREÇÃO: A URL agora aponta para a rota MVC correta para detalhes da barbearia. -->
                    <a href="/barbershop/details/${service.barberShopId}" class="btn btn-outline-light w-100">Ver Perfil da Barbearia</a>
                </div>
            </div>
        </div>
    `;
}
