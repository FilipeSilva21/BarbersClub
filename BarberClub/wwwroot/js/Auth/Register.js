document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('registerForm');
    if (!form) return;

    const errorMessageDiv = document.getElementById('errorMessage');
    const submitButton = form.querySelector('button[type="submit"]');
    const spinner = submitButton.querySelector('.spinner-border');
    const successModalElement = document.getElementById('successModal');

    if (!errorMessageDiv || !submitButton || !spinner || !successModalElement) {
        console.error('Um ou mais elementos do formulário de registro não foram encontrados no HTML.');
        return;
    }

    const successModal = new bootstrap.Modal(successModalElement);
    const redirectButton = document.getElementById('redirectButton');
    const loginUrl = form.dataset.loginUrl || '/api/auth/login';
    redirectButton.href = loginUrl;

    form.addEventListener('submit', async function (event) {
        event.preventDefault();
        errorMessageDiv.textContent = '';
        errorMessageDiv.style.display = 'none';

        const firstName = document.getElementById('firstName')?.value.trim();
        const lastName = document.getElementById('lastName')?.value.trim();
        const email = document.getElementById('email')?.value.trim();
        const password = document.getElementById('password')?.value;
        const confirmPassword = document.getElementById('confirmPassword')?.value;
        const role = "User";

        if (!firstName || !email || !password) {
            errorMessageDiv.textContent = 'Por favor, preencha todos os campos obrigatórios.';
            errorMessageDiv.style.display = 'block';
            return;
        }

        if (password !== confirmPassword) {
            errorMessageDiv.textContent = 'As senhas não conferem.';
            errorMessageDiv.style.display = 'block';
            return;
        }

        spinner.classList.remove('d-none');
        submitButton.disabled = true;

        const controller = new AbortController();
        const timeoutId = setTimeout(() => controller.abort(), 15000);

        const requestData = { firstName, lastName, email, password, confirmPassword, role };

        try {
            const response = await fetch('/api/auth/register', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(requestData),
                signal: controller.signal
            });

            clearTimeout(timeoutId);

            if (response.ok) {
                spinner.classList.add('d-none');
                submitButton.disabled = false;
                successModal.show();
            } else {
                const errorData = await response.json().catch(() => ({ message: "Ocorreu um erro no servidor." }));
                errorMessageDiv.textContent = errorData.message;
                errorMessageDiv.style.display = 'block';
            }
        } catch (error) {
            clearTimeout(timeoutId);

            if (error.name === 'AbortError') {
                errorMessageDiv.textContent = 'O servidor demorou muito para responder.';
            } else {
                console.error('Erro na requisição:', error);
                errorMessageDiv.textContent = 'Erro de conexão.';
            }
            errorMessageDiv.style.display = 'block';
        } finally {
            spinner.classList.add('d-none');
            submitButton.disabled = false;
        }
    });
});