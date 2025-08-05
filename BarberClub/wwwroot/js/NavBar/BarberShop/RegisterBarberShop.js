// Em wwwroot/js/NavBar/BarberShop/RegisterBarberShop.js

document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('barberShopForm');
    if (!form) return;

    const submitButton = form.querySelector('button[type="submit"]');
    const spinner = submitButton.querySelector('.spinner-border');
    const errorMessageDiv = document.getElementById('errorMessage');
    const successModalElement = document.getElementById('successModal');
    const successModal = new bootstrap.Modal(successModalElement);

    // --- LÓGICA DE UI ---
    // Habilita/desabilita os campos de preço com base no checkbox
    document.querySelectorAll('.service-checkbox').forEach(checkbox => {
        checkbox.addEventListener('change', function () {
            const priceInput = form.querySelector(`.service-price-input[data-service-type="${this.value}"]`);
            if (priceInput) {
                priceInput.disabled = !this.checked;
                if (!this.checked) {
                    priceInput.value = ''; // Limpa o valor se o serviço for desmarcado
                }
            }
        });
    });
    
    form.addEventListener('submit', async function (event) {
        event.preventDefault();

        submitButton.disabled = true;
        spinner.classList.remove('d-none');
        errorMessageDiv.textContent = '';
        errorMessageDiv.classList.add('d-none');

        const token = localStorage.getItem('jwt_token');
        if (!token) {
            errorMessageDiv.textContent = 'Você precisa estar logado para realizar esta ação.';
            errorMessageDiv.classList.remove('d-none');
            submitButton.disabled = false;
            spinner.classList.add('d-none');
            return;
        }

        // --- COLETA DE DADOS CORRIGIDA ---
        const offeredServices = [];
        // CORREÇÃO APLICADA AQUI: Busca pela classe '.service-checkbox' em vez do 'name'
        form.querySelectorAll('.service-checkbox:checked').forEach(checkbox => {
            const serviceType = checkbox.value;
            const priceInput = form.querySelector(`.service-price-input[data-service-type="${serviceType}"]`);

            if (priceInput) {
                const price = parseFloat(priceInput.value);
                // Adiciona à lista apenas se o preço for um número válido e maior que zero
                if (!isNaN(price) && price > 0) {
                    offeredServices.push({
                        serviceType: serviceType,
                        price: price
                    });
                }
            }
        });

        const workingDays = Array.from(form.querySelectorAll('input[name="WorkingDays"]:checked'))
            .map(cb => cb.value);

        const formObject = {
            name: form.querySelector('input[name="Name"]').value,
            address: form.querySelector('input[name="Address"]').value,
            city: form.querySelector('input[name="City"]').value,
            state: form.querySelector('input[name="State"]').value,
            whatsApp: form.querySelector('input[name="WhatsApp"]').value,
            instagram: form.querySelector('input[name="Instagram"]').value,
            openingHours: form.querySelector('input[name="OpeningHours"]').value,
            closingHours: form.querySelector('input[name="ClosingHours"]').value,
            offeredServices: offeredServices, 
            workingDays: workingDays
        };

        const actionUrl = '/api/barbershop/register';

        try {
            const response = await fetch(actionUrl, {
                method: 'POST',
                body: JSON.stringify(formObject),
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                }
            });

            if (response.ok) {
                // Não é comum um endpoint de registro retornar um novo token, mas mantendo a lógica.
                const responseData = await response.json().catch(() => ({}));
                if (responseData.token) {
                    localStorage.setItem('jwt_token', responseData.token);
                }
                successModal.show();

                const redirectButton = document.getElementById('redirectButton');
                if(redirectButton) {
                    redirectButton.addEventListener('click', () => {
                        window.location.href = '/navbar/dashboard'; // Redireciona para o dashboard
                    });
                }

            } else {
                const errorData = await response.json().catch(() => ({ message: 'Ocorreu um erro. Tente novamente.'}));
                let message = errorData.message;

                if (errorData && errorData.errors) {
                    message = Object.values(errorData.errors).flat().join('\n');
                }

                errorMessageDiv.textContent = message;
                errorMessageDiv.classList.remove('d-none');
            }
        } catch (error) {
            console.error('Erro de rede:', error);
            errorMessageDiv.textContent = 'Não foi possível conectar ao servidor.';
            errorMessageDiv.classList.remove('d-none');
        } finally {
            submitButton.disabled = false;
            spinner.classList.add('d-none');
        }
    });
});