document.addEventListener('DOMContentLoaded', function () {
    const imageUploadInput = document.getElementById('ProfilePicFile');
    const imagePreview = document.getElementById('image-preview');

    if (imageUploadInput && imagePreview) {
        imageUploadInput.addEventListener('change', function (event) {
            const file = event.target.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = function (e) {
                    imagePreview.src = e.target.result;
                }
                reader.readAsDataURL(file);
            }
        });
    }

    // Habilitar/desabilitar campos de preço
    const serviceCheckboxes = document.querySelectorAll('.service-checkbox');
    serviceCheckboxes.forEach(checkbox => {
        checkbox.addEventListener('change', function () {
            const serviceType = this.value;
            const priceInput = document.querySelector(`.service-price-input[data-service-type="${serviceType}"]`);
            if (priceInput) {
                priceInput.disabled = !this.checked;
                if (!this.checked) {
                    priceInput.value = '';
                }
            }
        });
    });
    
    // --- Lógica de Submissão do Formulário ---
    const form = document.getElementById('registerBarberShopForm');
    if (!form) return;

    const submitButton = form.querySelector('button[type="submit"]');
    const spinner = submitButton.querySelector('.spinner-border');
    const errorMessageDiv = document.getElementById('errorMessage');
    const successModalElement = document.getElementById('successModal');
    const successModal = new bootstrap.Modal(successModalElement);

    form.addEventListener('submit', async function (event) {
        event.preventDefault();

        // UI de carregamento
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

        const formData = new FormData(form);

        formData.delete('Services.Price');

        let serviceIndex = 0;
        serviceCheckboxes.forEach(checkbox => {
            if (checkbox.checked) {
                const priceInput = document.querySelector(`.service-price-input[data-service-type="${checkbox.value}"]`);
                formData.append(`OfferedServices[${serviceIndex}].ServiceType`, checkbox.value);
                formData.append(`OfferedServices[${serviceIndex}].Price`, priceInput.value || '0');
                serviceIndex++;
            }
        });


        try {
            const response = await fetch(form.action, {
                method: 'POST',
                body: formData, 
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });
            if (response.ok) {
                const responseData = await response.json();

                if (responseData.token) {
                    localStorage.setItem('jwt_token', responseData.token);
                    console.log("Token de sessão atualizado com sucesso!");
                } 

                successModal.show();
            } else {
                const errorData = await response.json().catch(() => ({ message: `Erro ${response.status}: ${response.statusText}` }));
                let message = errorData.message || 'Ocorreu um erro ao processar sua solicitação.';
                if (errorData.errors) {
                    message = Object.values(errorData.errors).flat().join('\n');
                }
                errorMessageDiv.textContent = message;
                errorMessageDiv.classList.remove('d-none');
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