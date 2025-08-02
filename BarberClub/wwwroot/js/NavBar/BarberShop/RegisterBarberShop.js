// Em wwwroot/js/NavBar/BarberShop/RegisterBarberShop.js

document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('barberShopForm');
    if (!form) return;

    const submitButton = form.querySelector('button[type="submit"]');
    const spinner = submitButton.querySelector('.spinner-border');
    const errorMessageDiv = document.getElementById('errorMessage');
    const successModalElement = document.getElementById('successModal');
    const successModal = new bootstrap.Modal(successModalElement);

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

        const offeredServices = Array.from(form.querySelectorAll('input[name="OfferedServices"]:checked'))
            .map(checkbox => checkbox.value);

        const formObject = {
            name: form.querySelector('input[name="Name"]').value,
            address: form.querySelector('input[name="Address"]').value,
            city: form.querySelector('input[name="City"]').value,
            state: form.querySelector('input[name="State"]').value,
            whatsApp: form.querySelector('input[name="WhatsApp"]').value,
            instagram: form.querySelector('input[name="Instagram"]').value,
            openingHours: form.querySelector('input[name="OpeningHours"]').value,
            closingHours: form.querySelector('input[name="ClosingHours"]').value,
            offeredServices: offeredServices
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
                const responseData = await response.json();
                if (responseData.token) {
                    localStorage.setItem('jwt_token', responseData.token);
                }
                successModal.show();
                successModalElement.addEventListener('hidden.bs.modal', function () {
                    window.location.href = '/NavBar/Dashboard';
                }, { once: true });
            } else {
                const errorData = await response.json();
                let message = 'Ocorreu um erro. Tente novamente.';

                if (errorData && errorData.errors) {
                    message = Object.values(errorData.errors).flat().join('\n');
                } else if (errorData && errorData.message) {
                    message = errorData.message;
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