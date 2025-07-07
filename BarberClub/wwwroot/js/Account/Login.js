document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('loginForm');
    if (!form) return;

    const errorMessageDiv = document.getElementById('errorMessage');
    const submitButton = form.querySelector('button[type="submit"]');
    const spinner = submitButton.querySelector('.spinner-border');
    if (spinner) spinner.style.display = 'inline-block';


    // Pega a URL de redirecionamento do atributo data no formulário
    const redirectUrl = form.dataset.redirectUrl;

    form.addEventListener('submit', async function (event) {
        event.preventDefault();
        errorMessageDiv.textContent = '';

        const email = document.getElementById('email').value;
        const password = document.getElementById('password').value;
        const requestData = { email, password };
        const spinner = submitButton ? submitButton.querySelector('.spinner-border') : null;

        try {
            const response = await fetch('/Account/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email, password })
            });

            if (response.ok) {
                const data = await response.json();
                localStorage.setItem('jwt_token', data.token);
                localStorage.setItem('user_name', data.user.name);
                window.location.href = '/';
            } else {
                errorMessageDiv.textContent = 'Email ou senha inválidos.';
            }
        } catch (error) {
            console.error('Erro de conexão:', error);
            errorMessageDiv.textContent = 'Erro de conexão com o servidor.';
        } finally {
            submitButton.disabled = false;
            spinner.style.display = 'none';
        }
    });
});