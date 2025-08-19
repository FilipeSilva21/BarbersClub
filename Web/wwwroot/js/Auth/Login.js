document.addEventListener('DOMContentLoaded', function () {
    const loginForm = document.getElementById('loginForm'); // Garanta que o ID do seu form é 'loginForm'
    const errorMessageDiv = document.getElementById('errorMessage');

    if (!loginForm) {
        console.error("O formulário de login não foi encontrado.");
        return;
    }

    loginForm.addEventListener('submit', async function (event) {
        event.preventDefault();
        errorMessageDiv.classList.add('d-none');

        const submitButton = loginForm.querySelector('button[type="submit"]');
        const spinner = submitButton.querySelector('.spinner-border');

        // Mostra o spinner e desativa o botão
        spinner.classList.remove('d-none');
        submitButton.disabled = true;

        const requestData = {
            email: document.getElementById('email').value,
            password: document.getElementById('password').value
        };

        try {
            const response = await fetch('/api/auth/login', { 
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(requestData)
            });

            if (response.ok) {
                const responseData = await response.json();
                const token = responseData.token;

                if (token) {
                    localStorage.setItem('jwt_token', token);

                    window.location.href = '/'; 
                } else {
                    throw new Error("Token não recebido do servidor.");
                }
            } else {
                // Trata erros de login (ex: email/senha inválidos)
                const errorData = await response.json().catch(() => ({ message: "Email ou senha inválidos." }));
                errorMessageDiv.textContent = errorData.message;
                errorMessageDiv.classList.remove('d-none');
            }
        } catch (error) {
            console.error('Erro de conexão:', error);
            errorMessageDiv.textContent = 'Erro de conexão. Tente novamente mais tarde.';
            errorMessageDiv.classList.remove('d-none');
        } finally {
            // Esconde o spinner e reativa o botão
            spinner.classList.add('d-none');
            submitButton.disabled = false;
        }
    });
});