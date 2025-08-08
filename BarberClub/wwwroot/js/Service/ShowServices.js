document.addEventListener('DOMContentLoaded', function () {
    const searchForm = document.getElementById('search-form');
    const resultsContainer = document.getElementById('results-container');
    const loadingMessage = document.getElementById('loading-message');

    function formatDate(dateString) {
        const options = { year: 'numeric', month: '2-digit', day: '2-digit', hour: '2-digit', minute: '2-digit' };
        return new Date(dateString).toLocaleDateString('pt-BR', options);
    }

    async function fetchAndRenderServices() {
        loadingMessage.style.display = 'block';
        resultsContainer.innerHTML = '';

        const barberShopName = document.getElementById('barberShopName').value;
        const serviceType = document.getElementById('ServiceType').value;
        const clientName = document.getElementById('clientName').value;
        const startDate = document.getElementById('startDate').value;
        const endDate = document.getElementById('endDate').value;

        const queryParams = new URLSearchParams({
            serviceType,
            barberShopName,
            clientName,
            startDate,
            endDate
        });

        const apiUrl = `/api/services?${queryParams}`;

        try {
            const response = await fetch(apiUrl);
            if (!response.ok) {
                throw new Error('Erro ao buscar os dados.');
            }
            const services = await response.json();

            loadingMessage.style.display = 'none';

            if (services.length === 0) {
                resultsContainer.innerHTML = '<p class="text-center">Nenhum serviço encontrado com os filtros aplicados.</p>';
                return;
            }

            services.forEach(service => {
                const serviceCard = `
                    <div class="col-md-6 col-lg-4">
                        <div class="card h-100 shadow-sm service-card">
                            <div class="card-body d-flex flex-column">
                                <h5 class="card-title fw-bold text-gold">${service.serviceType || 'Serviço não especificado'}</h5>
                                <p class="card-text mb-2"><i class="bi bi-calendar-event me-2"></i><strong>Data:</strong> ${formatDate(service.date)}</p>
                                <p class="card-text mb-2"><i class="bi bi-person me-2"></i><strong>Cliente:</strong> ${service.clientName || 'Não informado'}</p>
                                <p class="card-text mb-2"><i class="bi bi-shop me-2"></i><strong>Barbearia:</strong> ${service.barberShopName || 'Não informada'}</p>
                                <a href="/servicedetails/${service.serviceId}" class="btn btn-outline-gold mt-3">Ver Detalhes</a>
                            </div>
                        </div>
                    </div>
                `;
                resultsContainer.innerHTML += serviceCard;
            });

        } catch (error) {
            console.error('Erro:', error);
            loadingMessage.style.display = 'none';
            resultsContainer.innerHTML = '<p class="text-center text-danger">Ocorreu um erro ao carregar os serviços. Tente novamente mais tarde.</p>';
        }
    }

    searchForm.addEventListener('submit', function (event) {
        event.preventDefault();
        fetchAndRenderServices();
    });

    fetchAndRenderServices();
});