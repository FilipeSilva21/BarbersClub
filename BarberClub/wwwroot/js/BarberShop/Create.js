// Em wwwroot/js/BarberShop/Create.js

document.addEventListener('DOMContentLoaded', function () {
    // Seleciona os elementos do formulário
    const form = document.getElementById('barberShopForm');
    const submitButton = form.querySelector('button[type="submit"]');
    const spinner = submitButton.querySelector('.spinner-border');
    const errorMessageDiv = document.getElementById('errorMessage');
    const successModalElement = document.getElementById('successModal');
    const successModal = new bootstrap.Modal(successModalElement);

    // Adiciona um "ouvinte" para o evento de envio do formulário
    form.addEventListener('submit', async function (event) {
        // Impede o comportamento padrão de recarregar a página
        event.preventDefault();

        // 1. Prepara a UI para o envio (estado de loading)
        submitButton.disabled = true;
        spinner.classList.remove('d-none');
        errorMessageDiv.textContent = '';

        // Pega o token do localStorage
        const token = localStorage.getItem('jwt_token');

        if (!token) {
            errorMessageDiv.textContent = 'Você precisa estar logado para realizar esta ação.';
            submitButton.disabled = false;
            spinner.classList.add('d-none');
            return; // Para a execução
        }

        const formData = new FormData(form);
        const actionUrl = '/barbershopApi/register'; // URL da API

        // --- INÍCIO DA ALTERAÇÃO ---

        // 2. Converte os dados do formulário para um objeto JavaScript simples
        const formObject = {};
        formData.forEach((value, key) => {
            formObject[key] = value;
        });

        try {
            // 3. Envia os dados para a API como JSON
            const response = await fetch(actionUrl, {
                method: 'POST',
                // Converte o objeto para uma string JSON e o envia no corpo
                body: JSON.stringify(formObject),
                headers: {
                    // Define o Content-Type para application/json
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                }
            });

            // --- FIM DA ALTERAÇÃO ---

            if (response.ok) {
                // Se a resposta for SUCESSO (status 2xx)
                successModal.show(); // Mostra o modal de sucesso
            } else {
                // Se a resposta for ERRO (status 4xx ou 5xx)
                // Tenta ler a resposta de erro como JSON
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
            // Trata erros de rede (ex: servidor offline)
            console.error('Erro de rede:', error);
            errorMessageDiv.textContent = 'Não foi possível conectar ao servidor. Verifique sua conexão.';
        } finally {
            // 4. Restaura a UI ao estado normal
            submitButton.disabled = false;
            spinner.classList.add('d-none');
        }
    });
});