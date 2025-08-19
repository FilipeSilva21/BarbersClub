// wwwroot/js/service/registerservice

function getAuthHeaders() {
    const token = localStorage.getItem('jwt_token');
    if (!token) {
        window.location.href = '/Auth/Login';
        return {};
    }
    return {
        'Authorization': `Bearer ${token}`
    };
}

document.addEventListener('DOMContentLoaded', function () {

    const form = document.getElementById('registerServiceForm');
    const barberShopIdInput = document.getElementById('barberShopId');
    const userIdInput = document.getElementById('userId');
    const servicesSelect = document.getElementById('services');
    const priceInput = document.getElementById('price');
    const errorMessageDiv = document.getElementById('errorMessage');
    const submitButton = form.querySelector('button[type="submit"]');
    const spinner = submitButton.querySelector('.spinner-border');
    const buttonText = submitButton.querySelector('.button-text');
    const successModalElement = document.getElementById('successModal');
    const dateInput = document.getElementById('date');
    const timeSelect = document.getElementById('time');

    if (!form || !barberShopIdInput || !userIdInput || !servicesSelect || !priceInput || !errorMessageDiv || !submitButton || !dateInput || !timeSelect) {
        console.error('Um ou mais elementos essenciais do formulário não foram encontrados no DOM.');
        return;
    }
    
    const successModal = new bootstrap.Modal(successModalElement);

    if (typeof PRESELECTED_BARBERSHOP_ID !== 'undefined' && PRESELECTED_BARBERSHOP_ID) {
        barberShopIdInput.value = PRESELECTED_BARBERSHOP_ID;
    }
    if (typeof CURRENT_USER_ID !== 'undefined' && CURRENT_USER_ID) {
        userIdInput.value = CURRENT_USER_ID;
    }

    // Popula o dropdown de serviços
    if (typeof AVAILABLE_SERVICES !== 'undefined' && AVAILABLE_SERVICES.length > 0) {
        AVAILABLE_SERVICES.forEach(service => {
            const option = document.createElement('option');
            option.value = service.value;
            option.textContent = service.text;
            option.dataset.price = service.price; 
            servicesSelect.appendChild(option);
        });
    } else {
        servicesSelect.innerHTML = '<option value="" selected disabled>Nenhum serviço disponível</option>';
        servicesSelect.disabled = true;
    }

    servicesSelect.addEventListener('change', function() {
        const selectedOption = this.options[this.selectedIndex];
        const price = selectedOption.dataset.price;
        priceInput.value = price ? parseFloat(price).toFixed(2).replace('.', ',') : '';
    });

    // Define a data mínima como hoje
    const today = new Date().toISOString().split('T')[0];
    dateInput.setAttribute('min', today);

    // Evento para buscar os horários quando a data é alterada
    dateInput.addEventListener('change', async function() {
        const selectedDate = this.value;
        const barberShopId = barberShopIdInput.value;

        if (!selectedDate || !barberShopId) {
            timeSelect.innerHTML = '<option value="">Selecione uma data primeiro</option>';
            timeSelect.disabled = true;
            return;
        }

        // Feedback visual para o usuário
        timeSelect.disabled = true;
        timeSelect.innerHTML = '<option value="">Carregando horários...</option>';

        try {
            const response = await fetch(`/api/services/bookedTimes?barberShopId=${barberShopId}&date=${selectedDate}`);

            if (!response.ok) 
                throw new Error('Falha ao buscar horários da barbearia.');

            const scheduleInfo = await response.json();

            populateAvailableTimeSlots(scheduleInfo);

        } catch (error) {
            console.error('Erro ao buscar horários:', error);
            timeSelect.innerHTML = '<option value="">Erro ao carregar</option>';
            errorMessageDiv.textContent = 'Não foi possível carregar os horários. Tente novamente.';
            errorMessageDiv.classList.remove('d-none');
        }
    });
    
    function populateAvailableTimeSlots(scheduleInfo) {
        timeSelect.innerHTML = '';

        if (!scheduleInfo || !scheduleInfo.openingTime || !scheduleInfo.closingTime) {
            console.error("A API não retornou os horários de funcionamento esperados.");
            timeSelect.innerHTML = '<option value="" disabled>Horários indisponíveis</option>';
            timeSelect.disabled = true;
            return;
        }

        const startHour = parseInt(scheduleInfo.openingTime.split(':')[0]);
        const endHour = parseInt(scheduleInfo.closingTime.split(':')[0]);
        const bookedTimes = scheduleInfo.bookedTimes;

        let availableSlotsCount = 0;

        for (let hour = startHour; hour <= endHour; hour++) {
            const timeSlot = `${String(hour).padStart(2, '0')}:00`;

            const isBooked = bookedTimes.some(booked => booked.startsWith(timeSlot));

            if (!isBooked) {
                const option = document.createElement('option');
                option.value = timeSlot;
                option.textContent = timeSlot;
                timeSelect.appendChild(option);
                availableSlotsCount++;
            }
        }

        if (availableSlotsCount === 0) {
            timeSelect.innerHTML = '<option value="" disabled>Nenhum horário disponível</option>';
            timeSelect.disabled = true;
        } else {
            const defaultOption = '<option value="" selected disabled>Selecione um horário</option>';
            timeSelect.innerHTML = defaultOption + timeSelect.innerHTML;
            timeSelect.disabled = false;
        }
    }
    
    form.addEventListener('submit', async function(event) {
        event.preventDefault();
        errorMessageDiv.classList.add('d-none');

        const requestData = {
            barberShopId: barberShopIdInput.value,
            userId: userIdInput.value,
            date: dateInput.value,
            time: timeSelect.value,
            serviceTypes: servicesSelect.value,
            description: document.getElementById('description').value
        };

        if (!requestData.date || !requestData.time || !requestData.serviceTypes) {
            errorMessageDiv.textContent = 'Por favor, preencha a data, hora e tipo de serviço.';
            errorMessageDiv.classList.remove('d-none');
            return;
        }

        // Feedback visual de carregamento
        buttonText.textContent = 'Agendando...';
        spinner.classList.remove('d-none');
        submitButton.disabled = true;

        try {
            const response = await fetch('/api/services', {
                method: 'POST',
                headers: { ...getAuthHeaders(), 'Content-Type': 'application/json' },
                body: JSON.stringify(requestData)
            });

            if (response.ok) {
                form.reset(); 
                priceInput.value = '';
                timeSelect.innerHTML = '<option value="">Selecione uma data primeiro</option>';
                timeSelect.disabled = true;
                successModal.show(); 
            } else {
                const errorData = await response.json().catch(() => ({ message: 'Ocorreu um erro ao processar sua solicitação.' }));
                errorMessageDiv.textContent = errorData.title || errorData.message || 'Erro desconhecido.';
                errorMessageDiv.classList.remove('d-none');
            }
        } catch (error) {
            console.error('Erro no envio do formulário:', error);
            errorMessageDiv.textContent = 'Erro de conexão. Verifique sua internet e tente novamente.';
            errorMessageDiv.classList.remove('d-none');
        } finally {
            buttonText.textContent = 'Agendar Serviço';
            spinner.classList.add('d-none');
            submitButton.disabled = false;
        }
    });
});