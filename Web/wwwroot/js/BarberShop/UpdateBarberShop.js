document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('barberShopEditForm');
    if (!form) {
        console.error("O formulário com id 'barberShopEditForm' não foi encontrado.");
        return;
    }

    const submitButton = form.querySelector('button[type="submit"]');
    const spinner = submitButton ? submitButton.querySelector('.spinner-border') : null;
    const errorMessageDiv = document.getElementById('errorMessage');
    const profilePicInput = document.getElementById('ProfilePicFile');
    const imagePreview = document.getElementById('image-preview');

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

    form.addEventListener('submit', async function (event) {
        event.preventDefault();

        if (submitButton) submitButton.disabled = true;
        if (spinner) spinner.classList.remove('d-none');
        const token = localStorage.getItem('jwt_token');
        if (!token) {
            if (submitButton) submitButton.disabled = false;
            if (spinner) spinner.classList.add('d-none');
            return;
        }

        const formData = new FormData(form);

        const workingDays = Array.from(form.querySelectorAll('input[name="WorkingDays"]:checked')).map(cb => cb.value);
        formData.delete('WorkingDays');
        workingDays.forEach(day => {
            formData.append('WorkingDays', day);
        });

        const offeredServices = [];
        form.querySelectorAll('.service-checkbox:checked').forEach(checkbox => {
            const serviceType = checkbox.value;
            const priceInput = form.querySelector(`.service-price-input[data-service-type="${serviceType}"]`);
            const price = parseFloat(priceInput.value.replace(',', '.'));
            if (priceInput && !isNaN(price) && price >= 0) {
                offeredServices.push({ serviceType, price });
            }
        });

        for (let pair of formData.entries()) {
            if (pair[0].startsWith('OfferedServices')) {
                formData.delete(pair[0]);
            }
        }
        offeredServices.forEach((service, index) => {
            formData.append(`OfferedServices[${index}].ServiceType`, service.serviceType);
            formData.append(`OfferedServices[${index}].Price`, service.price);
        });

        const barberShopId = form.querySelector('input[name="BarberShopId"]').value;
        if (!formData.get('BarberShopId')) {
            formData.append('BarberShopId', barberShopId);
        }

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
                if (errorMessageDiv) {
                    let errorText = 'Ocorreu um erro ao atualizar.';
                    if (response.status === 400 && errorData.errors) {
                        errorText = Object.values(errorData.errors).flat().join('\n');
                    } else if (errorData.message) {
                        errorText = errorData.message;
                    }
                    errorMessageDiv.textContent = errorText;
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
            if (submitButton) submitButton.disabled = false;
            if (spinner) spinner.classList.add('d-none');
        }
    });

    const deleteButton = document.getElementById('deleteButton');
    if (deleteButton) {
        deleteButton.addEventListener('click', function (event) {
            event.preventDefault();

            const deleteUrl = this.href;
            const token = localStorage.getItem('jwt_token');

            Swal.fire({
                title: 'Você tem certeza?',
                text: "Esta ação não pode ser revertida!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Sim, deletar!',
                cancelButtonText: 'Cancelar'
            }).then(async (result) => {
                if (result.isConfirmed) {
                    try {
                        const response = await fetch(deleteUrl, {
                            method: 'DELETE',
                            headers: {
                                'Authorization': `Bearer ${token}`
                            }
                        });

                        if (response.ok) {
                            // ### INÍCIO DA CORREÇÃO ###
                            const responseData = await response.json();

                            if (responseData.token) {
                                localStorage.setItem('jwt_token', responseData.token);
                                console.log("Token de sessão atualizado após exclusão!");
                            }
                            // ### FIM DA CORREÇÃO ###

                            Swal.fire(
                                'Deletada!',
                                'Sua barbearia foi removida.',
                                'success'
                            ).then(() => {
                                window.location.href = '/';
                            });
                        } else {
                            const errorData = await response.json();
                            Swal.fire(
                                'Erro!',
                                errorData.message || 'Não foi possível deletar a barbearia.',
                                'error'
                            );
                        }
                    } catch (error) {
                        Swal.fire(
                            'Erro de Rede!',
                            'Não foi possível conectar ao servidor.',
                            'error'
                        );
                    }
                }
            });
        });
    }
});