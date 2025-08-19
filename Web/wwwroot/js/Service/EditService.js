document.addEventListener('DOMContentLoaded', function() {

    // --- PARTE 1: LÓGICA DE PRÉ-VISUALIZAÇÃO DA IMAGEM (continua igual) ---
    const photoInput = document.getElementById('photoFile');
    if (photoInput) {
        photoInput.addEventListener('change', function(event) {
            const imagePreview = document.getElementById('imagePreview');
            const file = event.target.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = function(e) {
                    imagePreview.src = e.target.result;
                    imagePreview.style.display = 'block';
                }
                reader.readAsDataURL(file);
            }
        });
    }

    // --- PARTE 2: NOVA LÓGICA PARA SALVAR ALTERAÇÕES (SUBMISSÃO DO FORMULÁRIO) ---
    const editForm = document.querySelector('form');

    if (editForm) {
        editForm.addEventListener('submit', async function(event) {
            // 1. Previne o envio padrão do formulário que recarrega a página
            event.preventDefault();

            const submitButton = this.querySelector('button[type="submit"]');
            submitButton.disabled = true; // Desabilita o botão para evitar cliques duplos

            const serviceId = this.querySelector('input[name="ServiceId"]').value;
            const formData = new FormData(this); // 2. Coleta TODOS os dados do formulário (incluindo a imagem)
            const token = localStorage.getItem('jwt_token');

            try {
                // 3. Envia os dados para o endpoint correto na API
                const response = await fetch(`/api/services/update/${serviceId}`, {
                    method: 'PUT',
                    body: formData,
                    headers: {
                        'Authorization': `Bearer ${token}`
                        // ATENÇÃO: Não defina 'Content-Type'. 
                        // O navegador faz isso automaticamente para multipart/form-data.
                    }
                });

                if (!response.ok) {
                    const errorData = await response.json();
                    throw new Error(errorData.message || 'Falha ao atualizar o agendamento.');
                }

                await Swal.fire('Sucesso!', 'Agendamento atualizado com sucesso.', 'success');

                // 4. Redireciona o usuário para a página de agendamentos da barbearia
                // A variável BARBERSHOP_ID deve ser definida no seu arquivo .cshtml
                window.location.href = `/services/barbershop/${BARBERSHOP_ID}`;

            } catch (error) {
                Swal.fire('Erro!', error.message || 'Não foi possível atualizar o agendamento.', 'error');
            } finally {
                submitButton.disabled = false; // Reabilita o botão
            }
        });
    }


    // --- PARTE 3: LÓGICA DE CANCELAMENTO (continua igual) ---
    const formContainer = document.querySelector('.col-md-8.col-lg-6');

    if (formContainer) {
        formContainer.addEventListener('click', function(event) {
            if (event.target && event.target.id === 'cancel-button') {
                const serviceId = event.target.dataset.serviceId;

                Swal.fire({
                    title: 'Tem certeza?',
                    text: "Esta ação não poderá ser revertida!",
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#d33',
                    cancelButtonColor: '#6c757d',
                    confirmButtonText: 'Sim, cancelar!',
                    cancelButtonText: 'Não'
                }).then((result) => {
                    if (result.isConfirmed) {
                        cancelService(serviceId);
                    }
                });
            }
        });
    }

    async function cancelService(serviceId) {
        const token = localStorage.getItem('jwt_token');
        try {
            const response = await fetch(`/api/services/cancel/${serviceId}`, {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error('Falha ao cancelar o agendamento.');
            }

            await Swal.fire(
                'Cancelado!',
                'O agendamento foi cancelado com sucesso.',
                'success'
            );

            window.location.href = '/navbar/dashboard';

        } catch (error) {
            Swal.fire(
                'Erro!',
                'Não foi possível cancelar o agendamento.',
                'error'
            );
        }
    }
});