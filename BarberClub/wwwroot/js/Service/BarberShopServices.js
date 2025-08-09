document.addEventListener('DOMContentLoaded', function () {
    // --- Referências aos elementos do DOM ---
    const appointmentListContainer = document.getElementById('appointment-list-container');
    const loadingSpinner = document.getElementById('loading-spinner');
    const photoUploader = document.getElementById('photoUploader');

    // --- Referências aos novos campos de filtro ---
    const clientNameFilter = document.getElementById('clientNameFilter');
    const serviceTypeFilter = document.getElementById('serviceTypeFilter');
    const startDateFilter = document.getElementById('startDateFilter');
    const endDateFilter = document.getElementById('endDateFilter');
    const timeFilter = document.getElementById('timeFilter');
    const filterButton = document.getElementById('filterButton');
    const clearFiltersButton = document.getElementById('clearFiltersButton');

    // Validação inicial do ID da barbearia
    if (typeof BARBERSHOP_ID === 'undefined') {
        console.error('ID da barbearia não foi definido na página.');
        if (loadingSpinner) loadingSpinner.style.display = 'none';
        appointmentListContainer.innerHTML = '<p class="text-center text-danger">Erro de configuração: ID da barbearia não encontrado.</p>';
        return;
    }

    // --- Função para criar os cards de agendamento (mantida) ---
    function createAppointmentCard(appt) {
        const dateOnly = appt.date.split('T')[0];
        const fullDateTime = new Date(`${dateOnly}T${appt.time}`);
        const formattedDate = fullDateTime.toLocaleDateString('pt-BR', { day: '2-digit', month: 'long' });
        const formattedTime = fullDateTime.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });

        const statusParaAcoes = ['Confirmado'];

        const actionButtons = statusParaAcoes.includes(appt.status)
            ? `
            <div class="mt-3 action-buttons">
                <button class="btn btn-sm btn-success btn-conclude" data-id="${appt.serviceId}">
                    <i class="bi bi-check-lg"></i> Concluir
                </button>
                <a href="/service/edit/${appt.serviceId}" class="btn btn-sm btn-outline-light btn-edit">
                    <i class="bi bi-pencil-square"></i> Editar
                </a>
            </div>`
            : '';

        return `
        <div class="card appointment-card mb-3">
            <div class="card-body">
                <div class="d-flex justify-content-between align-items-start">
                    <div>
                        <h5 class="card-title mb-1">${appt.clientName}</h5>
                        <p class="card-text text-muted mb-2">${appt.serviceType}</p>
                        <p class="card-text small">
                            <i class="bi bi-calendar-event me-2"></i>${formattedDate}
                            <i class="bi bi-clock ms-3 me-2"></i>${formattedTime}
                        </p>
                    </div>
                    <div class="text-end">
                        <span class="badge status-${appt.status?.toLowerCase()}">${appt.status || 'Agendado'}</span>
                        ${actionButtons}
                    </div>
                </div>
            </div>
        </div>`;
    }

    // --- Funções de Conclusão (mantidas) ---
    async function handleConcludeClick(serviceId) {
        Swal.fire({
            title: 'Concluir Agendamento',
            text: "Deseja adicionar uma foto do serviço realizado?",
            icon: 'question',
            showDenyButton: true,
            showCancelButton: true,
            confirmButtonText: '<i class="bi bi-camera-fill"></i> Sim, com Foto',
            denyButtonText: 'Apenas Concluir',
            cancelButtonText: 'Voltar'
        }).then((result) => {
            if (result.isConfirmed) {
                photoUploader.dataset.serviceId = serviceId;
                photoUploader.click();
            } else if (result.isDenied) {
                concludeServiceWithoutPhoto(serviceId);
            }
        });
    }

    photoUploader.addEventListener('change', function() {
        if (this.files && this.files.length > 0) {
            const file = this.files[0];
            const serviceId = this.dataset.serviceId;
            const formData = new FormData();
            formData.append('photoFile', file);
            uploadAndConclude(serviceId, formData);
        }
        this.value = '';
    });

    async function uploadAndConclude(serviceId, formData) {
        Swal.fire({
            title: 'Enviando foto...',
            text: 'Por favor, aguarde.',
            allowOutsideClick: false,
            didOpen: () => Swal.showLoading()
        });

        try {
            const token = localStorage.getItem('jwt_token');
            const response = await fetch(`/api/services/conclude-with-photo/${serviceId}`, {
                method: 'POST',
                headers: { 'Authorization': `Bearer ${token}` },
                body: formData
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || 'Falha ao enviar a foto.');
            }

            await Swal.fire('Sucesso!', 'O agendamento foi concluído e a foto foi salva.', 'success');
            fetchAppointments();
        } catch (error) {
            Swal.fire('Erro!', error.message, 'error');
        }
    }

    async function concludeServiceWithoutPhoto(serviceId) {
        try {
            const token = localStorage.getItem('jwt_token');
            const response = await fetch(`/api/services/conclude/${serviceId}`, {
                method: 'POST',
                headers: { 'Authorization': `Bearer ${token}` }
            });
            if (!response.ok) throw new Error('Falha ao concluir.');
            await Swal.fire('Sucesso!', 'O agendamento foi marcado como concluído.', 'success');
            fetchAppointments();
        } catch (error) {
            Swal.fire('Erro!', 'Não foi possível concluir o agendamento.', 'error');
        }
    }

    // --- Função principal de busca de dados ---
    async function fetchAppointments() {
        if (loadingSpinner) loadingSpinner.style.display = 'block';
        appointmentListContainer.innerHTML = '';

        // Constrói a URL com os parâmetros de filtro
        const params = new URLSearchParams();
        if (clientNameFilter.value) params.append('clientName', clientNameFilter.value);
        if (serviceTypeFilter.value) params.append('serviceType', serviceTypeFilter.value);
        if (startDateFilter.value) params.append('startDate', startDateFilter.value);
        if (endDateFilter.value) params.append('endDate', endDateFilter.value);
        if (timeFilter.value) params.append('time', timeFilter.value);

        const queryString = params.toString();
        const url = `/api/services/barbershop/${BARBERSHOP_ID}?${queryString}`;

        try {
            const token = localStorage.getItem('jwt_token');
            const response = await fetch(url, {
                headers: { 'Authorization': `Bearer ${token}` }
            });

            if (!response.ok) throw new Error('Falha ao buscar os agendamentos.');

            const appointments = await response.json();

            if (appointments.length === 0) {
                appointmentListContainer.innerHTML = `<div class="text-center p-5"><p class="text-muted">Nenhum agendamento encontrado para os filtros aplicados.</p></div>`;
            } else {
                appointmentListContainer.innerHTML = appointments.map(createAppointmentCard).join('');
            }
        } catch (error) {
            console.error(error);
            appointmentListContainer.innerHTML = '<p class="text-danger text-center">Não foi possível carregar os agendamentos.</p>';
        } finally {
            if (loadingSpinner) loadingSpinner.style.display = 'none';
        }
    }

    // --- Listeners de Eventos para os botões ---

    // Listener para o clique no botão "Filtrar"
    filterButton.addEventListener('click', fetchAppointments);

    // Listener para o clique no botão "Limpar Filtros"
    clearFiltersButton.addEventListener('click', () => {
        clientNameFilter.value = '';
        serviceTypeFilter.value = '';
        startDateFilter.value = '';
        endDateFilter.value = '';
        timeFilter.value = '';
        fetchAppointments(); // Busca novamente sem os filtros
    });

    // Listener principal para os botões de concluir dentro dos cards
    appointmentListContainer.addEventListener('click', function(e) {
        const concludeButton = e.target.closest('.btn-conclude');
        if (concludeButton) {
            const serviceId = concludeButton.dataset.id;
            handleConcludeClick(serviceId);
        }
    });

    // --- Inicia a busca de agendamentos ao carregar a página ---
    fetchAppointments();
});