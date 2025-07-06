// wwwroot/js/script.js

document.addEventListener('DOMContentLoaded', () => {

    // --- Lógica da Tela de Login ---
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', async (e) => {
            // 1. Impede o envio tradicional do formulário que recarrega a página
            e.preventDefault();

            const email = document.getElementById('email').value;
            const password = document.getElementById('password').value;
            const loginError = document.getElementById('loginError');
            loginError.classList.add('d-none'); // Esconde erros antigos

            try {
                // 2. Faz a chamada para a sua API
                const response = await fetch('/api/auth/login', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ email: email, password: password })
                });

                const data = await response.json();

                if (!response.ok) {
                    // Se a API retornou um erro (ex: BadRequest)
                    throw new Error(data.message || 'Erro ao tentar fazer login.');
                }
                
                // 3. Sucesso! Salva o token e redireciona
                console.log('Login bem-sucedido. Token:', data.token);
                localStorage.setItem('jwt_token', data.token);

                // Lógica de redirecionamento (precisaria decodificar o token para saber a role)
                // Por enquanto, vamos redirecionar para a home principal.
                // Idealmente, sua API de login poderia retornar a role junto com o token.
                window.location.href = '/Home'; // Redireciona para a Action "Index" do "HomeController"

            } catch (error) {
                // 4. Exibe o erro na tela
                loginError.textContent = error.message;
                loginError.classList.remove('d-none');
            }
        });
    }


    // --- Lógica da Tela de Registro ---
    const registerForm = document.getElementById('registerForm');
    if (registerForm) {
        // Lógica para o seletor de perfil (role) - continua a mesma
        const roleButtons = document.querySelectorAll('.btn-role-selector');
        const roleInput = document.getElementById('Role');

        roleButtons.forEach(button => {
            button.addEventListener('click', () => {
                roleButtons.forEach(btn => btn.classList.remove('active'));
                button.classList.add('active');
                if (roleInput) {
                    roleInput.value = button.getAttribute('data-role');
                }
            });
        });

        // Lógica de envio do formulário de registro
        registerForm.addEventListener('submit', async (e) => {
            e.preventDefault();

            const username = document.getElementById('Username').value;
            const email = document.getElementById('Email').value;
            const password = document.getElementById('Password').value;
            const confirmPassword = document.getElementById('ConfirmPassword').value;
            const role = document.getElementById('Role').value;
            const registerError = document.getElementById('registerError');
            registerError.classList.add('d-none');

            if (password !== confirmPassword) {
                alert("As senhas nao coincidem.");
                registerError.textContent = 'As senhas não coincidem.';
                registerError.classList.remove('d-none');
                return;
            }

            try {
                const response = await fetch('/api/auth/register', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ username, email, password, confirmPassword, role })
                });

                const data = await response.json();

                if (!response.ok) {
                    throw new Error(data.message || 'Erro ao registrar.');
                }

                alert('Conta registrada com sucesso! Você será redirecionado para a tela de login.');
                window.location.href = '/Account/Login'; // Redireciona para a Action "Login" do "AccountController"

            } catch (error) {
                registerError.textContent = error.message;
                registerError.classList.remove('d-none');
            }
        });
    }
});