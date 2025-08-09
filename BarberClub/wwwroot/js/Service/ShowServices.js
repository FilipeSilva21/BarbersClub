// Em /wwwroot/js/Service/ShowServices.js

document.addEventListener('DOMContentLoaded', function () {
    const searchForm = document.getElementById('search-form');
    const resultsContainer = document.getElementById('results-container');
    const loadingMessage = document.getElementById('loading-message');

    // Referências aos filtros
    const barberShopName = document.getElementById('barberShopName');
    const serviceType = document.getElementById('ServiceType');
    const clientName = document.getElementById('clientName');
    const startDate = document.getElementById('startDate');
    const endDate = document.getElementById('endDate');
    const hasPhotoFilter = document.getElementById('hasPhotoFilter');
    const sortByFilter = document.getElementById('sortByFilter');

    // Marca o checkbox "Apenas com foto" como padrão
    hasPhotoFilter.checked = true;

    function formatDate(dateString) {
        if (!dateString) return 'Data não informada';
        return new Date(dateString).toLocaleDateString('pt-BR', { day: '2-digit', month: 'long', year: 'numeric' });
    }

    async function fetchAndRenderServices() {
        loadingMessage.style.display = 'block';
        resultsContainer.innerHTML = '';

        // Pega os valores dos filtros usando as variáveis
        const queryParams = new URLSearchParams();
        if (serviceType.value) queryParams.append('serviceType', serviceType.value);
        if (barberShopName.value) queryParams.append('barberShopName', barberShopName.value);
        if (clientName.value) queryParams.append('clientName', clientName.value);
        if (startDate.value) queryParams.append('startDate', startDate.value);
        if (endDate.value) queryParams.append('endDate', endDate.value);
        if (hasPhotoFilter.checked) queryParams.append('hasPhoto', 'true');
        if (sortByFilter.value) queryParams.append('sortBy', sortByFilter.value);

        const apiUrl = `/api/services?${queryParams}`;

        try {
            const response = await fetch(apiUrl);
            if (!response.ok) throw new Error('Erro ao buscar os dados.');

            const services = await response.json();
            loadingMessage.style.display = 'none';

            if (services.length === 0) {
                resultsContainer.innerHTML = '<div class="col-12"><p class="text-center">Nenhum serviço encontrado com os filtros aplicados.</p></div>';
                return;
            }

            services.forEach(service => {
                const imageUrl = service.serviceImageUrl || service.ServiceImageUrl;

                // 2. Só criamos a tag <img> se a URL da imagem realmente existir.
                const imageHtml = imageUrl
                    ? `<img src="${imageUrl}" class="card-img-top service-feed-image" alt="Foto do serviço ${service.serviceType}">`
                    : '';

                const serviceCard = `
                <div class="col-md-10 col-lg-8 col-xl-7">
                    <div class="card shadow-sm service-feed-card mb-4">
                        <div class="card-header bg-white d-flex align-items-center">
                            <i class="bi bi-shop me-2 fs-5 text-gold"></i>
                            <a href="#" class="fw-bold text-decoration-none text-dark">${service.barberShopName}</a>
                        </div>
                        
                        ${imageHtml}
                        
                        <div class="card-body">
                            <p class="card-text">
                                <strong class="text-gold">${service.serviceType}</strong> para 
                                <strong>${service.clientName}</strong>.
                            </p>
                            <p class="card-text text-muted small">
                                ${new Date(service.date).toLocaleDateString('pt-BR', {day: '2-digit', month: 'long', year: 'numeric' })}
                            </p>
                        </div>
                        <div class="card-footer bg-white">
                             <a href="/Service/Edit/${service.serviceId}" class="btn btn-sm btn-outline-secondary">Ver Detalhes</a>
                        </div>
                    </div>
                </div>
            `;
                resultsContainer.innerHTML += serviceCard;
            });

        } catch (error) {
            console.error('Erro:', error);
            loadingMessage.style.display = 'none';
            resultsContainer.innerHTML = '<div class="col-12"><p class="text-center text-danger">Ocorreu um erro ao carregar os serviços.</p></div>';
        } 
    }

    searchForm.addEventListener('submit', function (event) {
        event.preventDefault();
        fetchAndRenderServices();
    });

    fetchAndRenderServices();
});