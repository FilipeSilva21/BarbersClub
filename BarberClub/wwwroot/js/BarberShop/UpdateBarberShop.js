// Em wwwroot/js/BarberShop/EditBarberShop.js

document.addEventListener('DOMContentLoaded', function () {
    // Aponta para o ID do formulário de edição
    const form = document.getElementById('barberShopEditForm');
    if (!form) {
        console.error("O formulário com id 'barberShopEditForm' não foi encontrado.");
        return;
    }

    // --- Seletores de Elementos ---
    const submitButton = form.querySelector('button[type="submit"]');
    const spinner = submitButton ? submitButton.querySelector('.spinner-border') : null;
    const errorMessageDiv = document.getElementById('errorMessage');
    const profilePicInput = document.getElementById('ProfilePicFile');
    const imagePreview = document.getElementById('image-preview');

    // --- LÓGICA DE UI (Interação com a Tela) ---

    // 1. Pré-visualização da imagem de perfil (igual ao de registro)
    if (profilePicInput && imagePreview) {
        profilePicInput.addEventListener('change', function (event) {
            const file = event.target.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = (e) => { imagePreview.src = e.target.result; };
                reader.readAsDataURL(file);
            }
        });
    }

    // 2. Habilita/desabilita os campos de preço (igual ao de registro)
    document.querySelectorAll('.service-checkbox').forEach(checkbox => {
        checkbox.addEventListener('change', function () {
            const serviceType = this.value;
            const priceInput = form.querySelector(`.service-price-input[data-service-type="${serviceType}"]`);
            if (priceInput) {
                priceInput.disabled = !this.checked;
                if (!this.checked) priceInput.value = '';
            }
        });
    });

    // --- LÓGICA DE ENVIO DO FORMULÁRIO (SUBMIT) ---
    form.addEventListener('submit', async function (event) {
        event.preventDefault(); // Impede o envio padrão do formulário

        if(submitButton) submitButton.disabled = true;
        if(spinner) spinner.classList.remove('d-none');
        const token = localStorage.getItem('jwt_token');
        if (!token) { /* ... trata erro de token ... */ return; }

        // Cria um FormData usando o formulário para pegar os campos simples automaticamente
        const formData = new FormData(form);

        // --- CORREÇÃO APLICADA AQUI ---

        // 1. Coleta os arrays complexos em JavaScript
        const workingDays = Array.from(form.querySelectorAll('input[name="WorkingDays"]:checked')).map(cb => cb.value);
        const offeredServices = [];
        form.querySelectorAll('.service-checkbox:checked').forEach(checkbox => {
            const serviceType = checkbox.value;
            const priceInput = form.querySelector(`.service-price-input[data-service-type="${serviceType}"]`);
            const price = parseFloat(priceInput.value.replace(',', '.'));
            if (priceInput && !isNaN(price) && price >= 0) {
                offeredServices.push({ serviceType, price });
            }
        });

        // 2. LIMPA os dados que o FormData pegou automaticamente para evitar duplicatas
        formData.delete('WorkingDays');
        formData.delete('OfferedServices.ServiceType');
        formData.delete('OfferedServices.Price');

        // 3. ADICIONA os dias de funcionamento no formato correto (múltiplos valores com a mesma chave)
        workingDays.forEach(day => {
            formData.append('WorkingDays', day);
        });

        // 4. ADICIONA os serviços oferecidos no formato indexado correto
        offeredServices.forEach((service, index) => {
            formData.append(`OfferedServices[${index}].ServiceType`, service.serviceType);
            formData.append(`OfferedServices[${index}].Price`, service.price);
        });

        const actionUrl = '/api/barbershop/edit';

        try {
            const response = await fetch(actionUrl, {
                method: 'POST',
                body: formData,
                headers: { 'Authorization': `Bearer ${token}` }
            });

            if (response.ok) {
                Swal.fire({
                    title: 'Sucesso!',
                    text: 'Sua barbearia foi atualizada.',
                    icon: 'success',
                    timer: 2000,
                    showConfirmButton: false
                }).then(() => {
                    window.location.href = '/NavBar/Dashboard';
                });
            } else {
                const errorData = await response.json().catch(() => ({ message: 'Ocorreu um erro ao atualizar.' }));
                if(errorMessageDiv) {
                    errorMessageDiv.textContent = errorData.message || Object.values(errorData.errors).flat().join('\n');
                    errorMessageDiv.classList.remove('d-none');
                }
            }
        } catch (error) {
            console.error('Erro de rede:', error);
            if(errorMessageDiv) {
                errorMessageDiv.textContent = 'Não foi possível conectar ao servidor.';
                errorMessageDiv.classList.remove('d-none');
            }
        } finally {
            if(submitButton) submitButton.disabled = false;
            if(spinner) spinner.classList.add('d-none');
        }
    });
});