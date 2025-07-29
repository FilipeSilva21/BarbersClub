// Em wwwroot/js/BarberShop/Create.js

document.addEventListener('DOMContentLoaded', function () {
    
    const form = document.getElementById('barberShopForm');
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

        const token = localStorage.getItem('jwt_token');

        if (!token) {
            errorMessageDiv.textContent = 'Você precisa estar logado para realizar esta ação.';
            submitButton.disabled = false;
            spinner.classList.add('d-none');
            return; 
        }

        const formData = new FormData(form);
        const actionUrl = '/barberShopApi/register'; // URL da API

        const formObject = {};
        formData.forEach((value, key) => {
            formObject[key] = value;
        });

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
                successModal.show(); 
            } else {
                try {
                    const errorData = await response.json();
                    let message = 'Ocorreu um erro. Tente novamente.';

                    if (errorData && errorData.errors) {
                        message = Object.values(errorData.errors).flat().join('\n');
                    } else if (errorData.message) {
                        message = errorData.message;
                    } else if (typeof errorData === 'string'){
                        message = errorData;
                    }
                    errorMessageDiv.textContent = message;
                } catch(e) {
                    errorMessageDiv.textContent = `Erro ${response.status}: ${response.statusText}`;
                }
            }
        } catch (error) {
            console.error('Erro de rede:', error);
            errorMessageDiv.textContent = 'Não foi possível conectar ao servidor. Verifique sua conexão.';
        } finally {
            submitButton.disabled = false;
            spinner.classList.add('d-none');
        }
    });
});