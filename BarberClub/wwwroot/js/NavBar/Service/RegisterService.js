// Em wwwroot/js/Service/RegisterService.js

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

    if (!form || !barberShopIdInput || !userIdInput || !servicesSelect || !priceInput || !errorMessageDiv || !submitButton) {
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

        // Ativa o estado de carregamento
        buttonText.textContent = 'Agendando...';
        spinner.classList.remove('d-none');
        submitButton.disabled = true;

        try {
            const response = await fetch('/api/services', {
                method: 'POST',
                headers: {
                    ...getAuthHeaders(),
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(requestData)
            });

            if (response.ok) {
                form.reset();
                priceInput.value = ''; 
                successModal.show();
            } else {
                const error = await response.json().catch(() => ({ message: 'Ocorreu um erro desconhecido.' }));
                errorMessageDiv.textContent = error.message;
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