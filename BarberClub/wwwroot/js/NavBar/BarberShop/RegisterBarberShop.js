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
        errorMessageDiv.classList.add('d-none');

        const token = localStorage.getItem('jwt_token');

        if (!token) {
            errorMessageDiv.textContent = 'Você precisa estar logado para realizar esta ação.';
            errorMessageDiv.classList.remove('d-none');
            submitButton.disabled = false;
            spinner.classList.add('d-none');
            return;
        }

        // --- INÍCIO DA ALTERAÇÃO ---

        // 1. Coleta os serviços marcados em um array.
        const offeredServices = Array.from(form.querySelectorAll('input[name="OfferedServices"]:checked'))
            .map(checkbox => checkbox.value);

        // 2. Monta o objeto de dados com os outros campos e o array de serviços.
        const formObject = {
            name: form.querySelector('input[name="Name"]').value,
            address: form.querySelector('input[name="Address"]').value,
            city: form.querySelector('input[name="City"]').value,
            state: form.querySelector('input[name="State"]').value,
            whatsApp: form.querySelector('input[name="WhatsApp"]').value,
            instagram: form.querySelector('input[name="Instagram"]').value,
            offeredServices: offeredServices // A propriedade deve ter o mesmo nome da sua classe C#
        };

        // --- FIM DA ALTERAÇÃO ---

        const actionUrl = '/barbershopApi/register';

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
                if (responseData.token) {
                    localStorage.setItem('jwt_token', responseData.token);
                }
                
                successModal.show();
            } else {
                try {
                    const errorData = await response.json();
                    let message = 'Ocorreu um erro. Tente novamente.';

                    if (errorData && errorData.errors) {
                        message = Object.values(errorData.errors).flat().join('\n');
                    } else if (errorData && errorData.message) {
                        message = errorData.message;
                    } else if (typeof errorData === 'string'){
                        message = errorData;
                    }
                    errorMessageDiv.textContent = message;
                    errorMessageDiv.classList.remove('d-none');
                } catch(e) {
                    errorMessageDiv.textContent = `Erro ${response.status}: Ocorreu uma falha ao processar a requisição.`;
                    errorMessageDiv.classList.remove('d-none');
                }
            }
        } catch (error) {
            console.error('Erro de rede:', error);
            errorMessageDiv.textContent = 'Não foi possível conectar ao servidor. Verifique sua conexão.';
            errorMessageDiv.classList.remove('d-none');
        } finally {
            submitButton.disabled = false;
            spinner.classList.add('d-none');
        }
    });
});