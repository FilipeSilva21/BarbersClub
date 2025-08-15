// Em /wwwroot/js/Service/ShowServices.js

document.addEventListener('DOMContentLoaded', function () {
    const searchForm = document.getElementById('search-form');
    const resultsContainer = document.getElementById('results-container');
    const loadingMessage = document.getElementById('loading-message');

    // Referências aos filtros (já estavam corretas)
    const barberShopName = document.getElementById('barberShopName');
    const barber = document.getElementById('barber');
    const serviceType = document.getElementById('ServiceType');
    const startDate = document.getElementById('startDate');
    const endDate = document.getElementById('endDate');
    const hasPhotoFilter = document.getElementById('hasPhotoFilter');
    const sortByFilter = document.getElementById('sortByFilter');

    // Marca o checkbox "Apenas com foto" como padrão
    hasPhotoFilter.checked = true;

    // Em /wwwroot/js/Service/ShowServices.js

    function renderServices(services) {
        if (!services || services.length === 0) {
            resultsContainer.innerHTML = '<div class="col-12"><p class="text-center">Nenhum serviço encontrado com os filtros aplicados.</p></div>';
            return;
        }

        const cardsHtml = services.map(service => {
            const imageUrl = service.serviceImageUrl || '/images/default-service.png';
            const serviceDate = new Date(service.date).toLocaleDateString('pt-BR', { day: '2-digit', month: 'long', year: 'numeric' });
            const barberShopDetailsUrl = `/barbershop/details/${service.barberShopId}`;

            return `
            <div class="col-md-6 col-lg-4">
                <div class="card service-card h-100 shadow-sm">
                    <img src="${imageUrl}" class="card-img-top" alt="Foto do serviço ${service.serviceType}">
                    <div class="card-body d-flex flex-column">
                        <p class="card-text mb-2">
                            <a href="${barberShopDetailsUrl}" class="fw-bold text-decoration-none">${service.barberShopName}</a>
                        </p>
                        <h5 class="card-title fw-bold text-gold">${service.serviceType}</h5>
                        <p class="card-text">Feito por <strong>${service.barber}</strong>.</p>
                        <p class="card-text text-muted small mt-auto">
                            <i class="bi bi-calendar-event"></i> ${serviceDate}
                        </p>
                    </div>
                    <div class="card-footer text-center">
                         <a href="${barberShopDetailsUrl}" class="btn btn-sm btn-outline-gold w-100">
                             <i class="bi bi-shop me-1"></i> Ir para a barbearia
                         </a>
                    </div>
                </div>
            </div>
        `;
        }).join('');

        resultsContainer.innerHTML = cardsHtml;
    }
    async function fetchAndRenderServices() {
        loadingMessage.style.display = 'block';
        resultsContainer.innerHTML = '';

        const queryParams = new URLSearchParams();
        if (serviceType.value) queryParams.append('serviceType', serviceType.value);
        if (barberShopName.value) queryParams.append('barberShopName', barberShopName.value);
        if (startDate.value) queryParams.append('startDate', startDate.value);
        if (endDate.value) queryParams.append('endDate', endDate.value);
        if (hasPhotoFilter.checked) queryParams.append('hasPhoto', 'true');
        if (sortByFilter.value) queryParams.append('sortBy', sortByFilter.value);
        if (barber.value) queryParams.append('barber', barber.value);

        const apiUrl = `/api/services?${queryParams}`;

        try {
            const response = await fetch(apiUrl);
            if (!response.ok) throw new Error('Erro ao buscar os dados.');

            const services = await response.json();
            renderServices(services);

        } catch (error) {
            console.error('Erro:', error);
            resultsContainer.innerHTML = '<div class="col-12"><p class="text-center text-danger">Ocorreu um erro ao carregar os serviços.</p></div>';
        } finally {
            loadingMessage.style.display = 'none';
        }
    }

    searchForm.addEventListener('submit', function (event) {
        event.preventDefault();
        fetchAndRenderServices();
    });

    fetchAndRenderServices();
});