document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('registerForm');
    if (!form) return;

    const errorMessageDiv = document.getElementById('errorMessage');
    const submitButton = form.querySelector('button[type="submit"]');
    const spinner = submitButton.querySelector('.spinner-border');
    const successModal = new bootstrap.Modal(document.getElementById('successModal'));
    const redirectButton = document.getElementById('redirectButton');

    const loginUrl = form.dataset.loginUrl || '/Account/Login';
    redirectButton.href = loginUrl;

    form.addEventListener('submit', async function (event) {
        event.preventDefault();
        errorMessageDiv.textContent = '';

        const firstName = document.getElementById('username').value.trim();
        const lastName = ""; // Se quiser, adicione um campo no HTML
        const email = document.getElementById('email').value.trim();
        const password = document.getElementById('password').value;
        const confirmPassword = document.getElementById('confirmPassword').value;
        const role = document.querySelector('input[name="role"]:checked')?.value || "User";

        if (password !== confirmPassword) {
            errorMessageDiv.textContent = 'As senhas não conferem.';
            return;
        }

        const requestData = {
            firstName,
            lastName,
            email,
            password,
            confirmPassword,
            role
        };

        submitButton.disabled = true;
        if (spinner) spinner.style.display = 'inline-block';

        try {
            const response = await fetch('/auth/auth/register', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(requestData)
            });

            if (response.ok) {
                successModal.show();
            } else {
                const errorData = await response.json();
                errorMessageDiv.textContent = errorData.message || 'Não foi possível realizar o cadastro.';
            }
        } catch (error) {
            console.error('Erro de conexão:', error);
            errorMessageDiv.textContent = 'Erro de conexão com o servidor.';
        } finally {
            submitButton.disabled = false;
            if (spinner) spinner.style.display = 'none';
        }
    });
});
