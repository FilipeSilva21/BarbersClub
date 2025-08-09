document.getElementById('photoFile').addEventListener('change', function(event) {
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

const cancelButton = document.getElementById('cancel-button');

if (cancelButton) {
    cancelButton.addEventListener('click', function() {
        const serviceId = this.dataset.serviceId;

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