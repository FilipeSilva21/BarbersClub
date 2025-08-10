// Sua função de autenticação (sem alterações)
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
    // Suas constantes de elementos existentes
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

        if (price) {
            priceInput.value = parseFloat(price).toFixed(2).replace('.', ',');
        } else {
            priceInput.value = '';
        }
    });

    // Define a data mínima como hoje
    const today = new Date().toISOString().split('T')[0];
    dateInput.setAttribute('min', today);

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
            // Chamada para a API que criamos
            const response = await fetch(`/api/services/bookedTimes?barberShopId=${barberShopId}&date=${selectedDate}`);
            if (!response.ok) throw new Error('Falha ao buscar horários.');

            const bookedTimes = await response.json();
            populateAvailableTimeSlots(bookedTimes);
        } catch (error) {
            console.error('Erro ao buscar horários:', error);
            timeSelect.innerHTML = '<option value="">Erro ao carregar</option>';
        }
    });

    function populateAvailableTimeSlots(bookedTimes) {
        timeSelect.innerHTML = '';
        const operatingHours = { start: 8, end: 18 };
        let availableSlotsCount = 0;

        for (let hour = operatingHours.start; hour <= operatingHours.end; hour++) {
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
            timeSelect.innerHTML = `<option value="" selected disabled>Selecione um horário</option>${timeSelect.innerHTML}`;
            timeSelect.disabled = false;
        }
    }
    form.addEventListener('submit', async function(event) {
        event.preventDefault();
        errorMessageDiv.classList.add('d-none');

        const requestData = {
            barberShopId: barberShopIdInput.value,
            userId: userIdInput.value,
            date: document.getElementById('date').value,
            time: document.getElementById('time').value,
            services: servicesSelect.value,
            description: document.getElementById('description').value
        };

        if (!requestData.date || !requestData.time || !requestData.services) {
            errorMessageDiv.textContent = 'Por favor, preencha a data, hora e tipo de serviço.';
            errorMessageDiv.classList.remove('d-none');
            return;
        }

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
                const errorData = await response.json().catch(() => ({ message: 'Ocorreu um erro ao agendar.' }));
                errorMessageDiv.textContent = errorData.title || errorData.message || 'Erro desconhecido.';
                errorMessageDiv.classList.remove('d-none');
            }
        } catch (error) {
            console.error('Erro no envio:', error);
            errorMessageDiv.textContent = 'Erro de conexão. Tente novamente.';
            errorMessageDiv.classList.remove('d-none');
        } finally {
            buttonText.textContent = 'Agendar Serviço';
            spinner.classList.add('d-none');
            submitButton.disabled = false;
        }
    });
});