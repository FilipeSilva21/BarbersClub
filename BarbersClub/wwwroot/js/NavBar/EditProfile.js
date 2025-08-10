// wwwroot/js/User/EditProfile.js
// EditProfile.js - intercepta submit e envia PUT /api/users/edit com FormData
console.log('EditProfile.js loaded');

document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('userProfileEditForm');
    if (!form) {
        console.error("Form 'userProfileEditForm' não encontrado.");
        return;
    }
    console.log('form found');

    const submitButton = form.querySelector('button[type="submit"]');
    const spinner = submitButton ? submitButton.querySelector('.spinner-border') : null;
    const errorMessageDiv = document.getElementById('errorMessage');
    const profilePicInput = document.getElementById('ProfilePicFile');
    const imagePreview = document.getElementById('image-preview');

    // preview de imagem (opcional)
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

    // util: pega token antiforgery se existir no form
    function getAntiForgeryToken() {
        const antiInput = form.querySelector('input[name="__RequestVerificationToken"]');
        return antiInput ? antiInput.value : null;
    }

    // util: remove campos vazios do FormData
    function pruneEmptyFields(fd, keys) {
        for (const key of keys) {
            const val = fd.get(key);
            // Se é string vazia ou null/undefined remove
            if (val === '' || val == null) {
                fd.delete(key);
            }
            // Se for File e tamanho 0, considera vazio
            if (val instanceof File && val.size === 0) {
                fd.delete(key);
            }
        }
    }

    form.addEventListener('submit', async function (e) {
        e.preventDefault(); // essencial para evitar envio padrão (GET/POST)
        console.log('submit intercepted');

        if (submitButton) submitButton.disabled = true;
        if (spinner) spinner.classList.remove('d-none');
        if (errorMessageDiv) { errorMessageDiv.classList.add('d-none'); errorMessageDiv.textContent = ''; }

        try {
            const formData = new FormData(form);

            // Remover campos de senha vazios para não disparar validação [Required] no servidor
            pruneEmptyFields(formData, ['NewPassword', 'ConfirmPassword']);

            // Se nenhum arquivo selecionado, remova ProfilePicFile (evita enviar File vazio)
            const fileInput = profilePicInput;
            if (fileInput && (!fileInput.files || fileInput.files.length === 0 || (fileInput.files[0] && fileInput.files[0].size === 0))) {
                formData.delete('ProfilePicFile');
            }

            // Debug: listar pares (arquivo -> mostra nome/size)
            for (const pair of formData.entries()) {
                if (pair[1] instanceof File) {
                    console.log('FormData:', pair[0], 'File{name:', pair[1].name, 'size:', pair[1].size + '}');
                } else {
                    console.log('FormData:', pair[0], pair[1]);
                }
            }

            const token = localStorage.getItem('jwt_token'); // opcional, se você usa JWT
            const actionUrl = form.getAttribute('action') || '/api/users/edit';
            const headers = {};

            // se existir antiforgery token no form, envie também via header (alguns setups esperam isso)
            const anti = getAntiForgeryToken();
            if (anti) headers['RequestVerificationToken'] = anti;

            if (token) headers['Authorization'] = `Bearer ${token}`;

            // NÃO setar Content-Type — o browser define multipart boundary automaticamente
            const res = await fetch(actionUrl, {
                method: 'PUT', // corresponde ao [HttpPut("edit")] do seu UserApiController
                body: formData,
                headers
            });

            console.log('fetch done, status=', res.status);

            if (res.ok) {
                // sucesso — feedback e redirecionamento para perfil
                if (typeof Swal !== 'undefined') {
                    Swal.fire({ icon: 'success', title: 'Perfil atualizado!', timer: 1200, showConfirmButton: false })
                        .then(() => window.location.href = '/profile');
                } else {
                    alert('Perfil atualizado!');
                    window.location.href = '/profile';
                }
                return;
            }

            // tratar resposta de erro: tenta JSON então texto
            let errText = `Status ${res.status}`;
            try {
                const ct = res.headers.get('content-type') || '';
                if (ct.includes('application/json')) {
                    const json = await res.json();
                    if (json.errors) {
                        // Aggregate ModelState-like errors
                        const all = [];
                        for (const k in json.errors) {
                            if (Array.isArray(json.errors[k])) all.push(...json.errors[k]);
                            else all.push(json.errors[k]);
                        }
                        errText = all.join('\n');
                    } else if (json.message) {
                        errText = json.message;
                    } else {
                        errText = JSON.stringify(json);
                    }
                } else {
                    const text = await res.text();
                    // tenta extrair mensagem curta do HTML de validação (fallback)
                    const match = text.match(/<li[^>]*>(.*?)<\/li>/i);
                    errText = match ? match[1].replace(/<[^>]+>/g, '').trim() : text;
                }
            } catch (parseErr) {
                console.error('Erro ao parsear resposta de erro', parseErr);
            }

            console.error('Server error:', errText);
            if (errorMessageDiv) { errorMessageDiv.textContent = errText; errorMessageDiv.classList.remove('d-none'); }
            else alert(errText);

        } catch (networkErr) {
            console.error('Network error', networkErr);
            if (errorMessageDiv) { errorMessageDiv.textContent = 'Erro de rede'; errorMessageDiv.classList.remove('d-none'); }
            else alert('Erro de rede');
        } finally {
            if (submitButton) submitButton.disabled = false;
            if (spinner) spinner.classList.add('d-none');
        }
    });
});
