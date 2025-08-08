document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('registerForm');
    if (!form) return;

    // Elementos do formulário
    const errorMessageDiv = document.getElementById('errorMessage');
    const submitButton = form.querySelector('button[type="submit"]');
    const spinner = submitButton.querySelector('.spinner-border');

    // --- LÓGICA DA FOTO DE PERFIL ---
    const profilePicInput = document.getElementById('ProfilePic');
    const imagePreview = document.getElementById('image-preview');

    // Event listener para pré-visualização da imagem
    profilePicInput.addEventListener('change', function(event) {
        const file = event.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function(e) {
                imagePreview.src = e.target.result;
            }
            reader.readAsDataURL(file);
        }
    });

    // ... (sua lógica de modal permanece a mesma) ...
    const successModalElement = document.getElementById('successModal');
    const successModal = new bootstrap.Modal(successModalElement);

    form.addEventListener('submit', async function (event) {
        event.preventDefault();
        // ... (sua lógica de UI e validação inicial permanece a mesma) ...
        errorMessageDiv.textContent = '';
        errorMessageDiv.style.display = 'none';

        if (document.getElementById('password')?.value !== document.getElementById('confirmPassword')?.value) {
            errorMessageDiv.textContent = 'As senhas não conferem.';
            errorMessageDiv.style.display = 'block';
            return;
        }

        spinner.classList.remove('d-none');
        submitButton.disabled = true;

        // --- MUDANÇA PRINCIPAL: USANDO FORMDATA ---
        // --- BLOCO CORRIGIDO ---
        const formData = new FormData();

// Os IDs agora correspondem aos gerados pelo asp-for (ex: "FirstName", "Email", etc.)
        formData.append('FirstName', document.getElementById('FirstName')?.value.trim());
        formData.append('LastName', document.getElementById('LastName')?.value.trim());
        formData.append('Email', document.getElementById('Email')?.value.trim());
        formData.append('Password', document.getElementById('Password')?.value);
        formData.append('ConfirmPassword', document.getElementById('ConfirmPassword')?.value);
        formData.append('Role', "User");

// O código para a foto de perfil já estava correto, pois usava o ID com 'P' maiúsculo
        const profilePicInput = document.getElementById('ProfilePic');
        if (profilePicInput && profilePicInput.files[0]) {
            formData.append('ProfilePic', profilePicInput.files[0]);
        }

        try {
            // --- MUDANÇA NA REQUISIÇÃO FETCH ---
            const response = await fetch('/api/auth/register', {
                method: 'POST',
                // O corpo agora é o objeto FormData
                body: formData,
                // REMOVA o header 'Content-Type'. O navegador o definirá
                // automaticamente como 'multipart/form-data' com o boundary correto.
            });

            if (response.ok) {
                successModal.show();
            } else {
                const errorData = await response.json().catch(() => ({ message: "Ocorreu um erro no servidor." }));
                errorMessageDiv.textContent = errorData.message || 'Não foi possível realizar o cadastro.';
                errorMessageDiv.style.display = 'block';
            }
        } catch (error) {
            console.error('Erro na requisição:', error);
            errorMessageDiv.textContent = 'Erro de conexão.';
            errorMessageDiv.style.display = 'block';
        } finally {
            spinner.classList.add('d-none');
            submitButton.disabled = false;
        }
    });
});