document.addEventListener('DOMContentLoaded', function () {
    const appointmentListContainer = document.getElementById('appointment-list-container');
    const loadingSpinner = document.getElementById('loading-spinner');
    const tabs = document.querySelectorAll('.appointment-tabs .nav-link');

    function createAppointmentCard(appt) {
        const dateOnly = appt.date.split('T')[0]; 
        const fullDateTimeString = `${dateOnly}T${appt.time}`; 
        const date = new Date(fullDateTimeString);
        const formattedDate = date.toLocaleDateString('pt-BR', { day: '2-digit', month: 'long', year: 'numeric' });
        const formattedTime = date.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' });

        return `
        <div class="card appointment-card mb-3" data-id="${appt.serviceId}">
            <div class="card-body">
                <div class="d-flex justify-content-between align-items-start">
                    <div>
                        <h5 class="card-title mb-1">${appt.barberShopName}</h5>
                        <p class="card-text text-muted mb-2">${appt.serviceType}</p>
                        
                        <p class="card-text small"><i class="fas fa-calendar-alt me-2"></i>${formattedDate}<i class="fas fa-clock ms-3 me-2"></i>${formattedTime}</p>
                    </div>
                    <div class="text-end">
                        <span class="badge badge-status-${appt.status.toLowerCase()}">${appt.status}</span>
                        ${appt.status.toLowerCase() === 'confirmado' ? `<div class="mt-2"><button class="btn btn-sm btn-outline-danger btn-cancel" data-id="${appt.serviceId}">Cancelar</button></div>` : ''}
                        
                        ${
                            appt.status.toLowerCase() === 'concluido'
                                ? (appt.hasRating
                                        ? `<div class="mt-2"><span class="text-success small"><i class="fas fa-check-circle me-1"></i>Serviço já avaliado</span></div>`
                                        : `<div class="mt-2"><a href="/rate/${appt.serviceId}" class="btn btn-sm btn-outline-gold">Avaliar Serviço</a></div>`
                                )
                                : ''
                        }
                    </div>
                </div>
            </div>
        </div>`;
    }

    async function loadAppointments(status) {
        loadingSpinner.style.display = 'block';
        appointmentListContainer.innerHTML = '';

        try {
            const response = await fetch(`/api/services/myServices?status=${status}`);

            if (!response.ok) {
                if (response.status === 401) window.location.href = '/login';
                throw new Error('Falha ao buscar os agendamentos.');
            }

            const appointments = await response.json();

            if (appointments.length === 0) {
                appointmentListContainer.innerHTML = `<div class="text-center p-5 bg-dark-2 rounded"><p class="mb-0">Nenhum agendamento encontrado para este status.</p></div>`;
            } else {
                appointmentListContainer.innerHTML = appointments.map(createAppointmentCard).join('');
            }

        } catch (error) {
            console.error(error);
            appointmentListContainer.innerHTML = '<p class="text-danger text-center">Não foi possível carregar seus agendamentos.</p>';
        } finally {
            loadingSpinner.style.display = 'none';
        }
    }

    async function cancelAppointment(serviceId) {
        const result = await Swal.fire({
            title: 'Tem certeza?',
            text: "Você não poderá reverter esta ação!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: 'Sim, cancelar!',
            cancelButtonText: 'Manter agendamento'
        });

        if (result.isConfirmed) {
            try {
                const response = await fetch(`/api/services/cancel/${serviceId}`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                });

                if (!response.ok) throw new Error('Falha ao cancelar o agendamento.');

                Swal.fire('Cancelado!', 'Seu agendamento foi cancelado.', 'success');

                const activeStatus = document.querySelector('.appointment-tabs .nav-link.active').dataset.status;
                loadAppointments(activeStatus);

            } catch (error) {
                Swal.fire('Erro!', 'Não foi possível cancelar o agendamento.', 'error');
            }
        }
    }

    tabs.forEach(tab => {
        tab.addEventListener('click', function (e) {
            e.preventDefault();
            tabs.forEach(t => t.classList.remove('active'));
            this.classList.add('active');
            loadAppointments(this.dataset.status);
        });
    });

    appointmentListContainer.addEventListener('click', function(e) {
        if (e.target && e.target.classList.contains('btn-cancel')) {
            cancelAppointment(e.target.dataset.id);
        }
    });

    const initialStatus = document.querySelector('.appointment-tabs .nav-link.active')?.dataset.status || 'Confirmado';
    loadAppointments(initialStatus);
});