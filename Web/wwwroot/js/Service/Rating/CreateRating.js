// /js/Service/Rating/CreateRating.js

document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('ratingForm');
    if (!form) {
        console.error("Formulário 'ratingForm' não foi encontrado.");
        return;
    }

    const submitButton = form.querySelector('button[type="submit"]');
    const spinner = submitButton ? submitButton.querySelector('.spinner-border') : null;
    const buttonText = submitButton ? submitButton.querySelector('.button-text') : null;
    const errorMessageDiv = document.getElementById('errorMessage');
    const successModalElement = document.getElementById('ratingSuccessModal');
    const successModal = successModalElement ? new bootstrap.Modal(successModalElement) : null;

    form.addEventListener('submit', async function (event) {
        event.preventDefault(); // Impede o envio tradicional do formulário

        // Esconde erros antigos e mostra o spinner de carregamento
        errorMessageDiv.classList.add('d-none');
        errorMessageDiv.textContent = '';
        submitButton.disabled = true;
        spinner.classList.remove('d-none');
        buttonText.textContent = 'Enviando...';

        try {
            const formData = new FormData(form);

            const apiUrl = '/api/ratings';

            const response = await fetch(apiUrl, {
                method: 'POST', 
                body: formData,
            });

            if (response.ok) {
                if (successModal) {
                    successModal.show();
                    successModalElement.addEventListener('hidden.bs.modal', function () {
                        window.location.href = '/profile';
                    }, { once: true });
                } else {
                    alert('Avaliação enviada com sucesso!');
                    window.location.href = '/meus-agendamentos';
                }
            } else {
                const errorData = await response.json();
                const message = errorData.message || 'Ocorreu um erro ao enviar sua avaliação.';
                errorMessageDiv.textContent = message;
                errorMessageDiv.classList.remove('d-none');
            }

        } catch (error) {
            console.error('Erro na requisição fetch:', error);
            errorMessageDiv.textContent = 'Erro de conexão. Por favor, tente novamente.';
            errorMessageDiv.classList.remove('d-none');
        } finally {
            submitButton.disabled = false;
            spinner.classList.add('d-none');
            buttonText.textContent = 'Enviar Avaliação';
        }
    });
});