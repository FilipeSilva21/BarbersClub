// wwwroot/js/User/EditProfile.js

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

    // preview de imagem 
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

    function getAntiForgeryToken() {
        const antiInput = form.querySelector('input[name="__RequestVerificationToken"]');
        return antiInput ? antiInput.value : null;
    }

    function pruneEmptyFields(fd, keys) {
        for (const key of keys) {
            const val = fd.get(key);
            if (val === '' || val == null) {
                fd.delete(key);
            }
            if (val instanceof File && val.size === 0) {
                fd.delete(key);
            }
        }
    }

    form.addEventListener('submit', async function (e) {
        e.preventDefault();
        console.log('submit intercepted');

        if (submitButton) submitButton.disabled = true;
        if (spinner) spinner.classList.remove('d-none');
        if (errorMessageDiv) { errorMessageDiv.classList.add('d-none'); errorMessageDiv.textContent = ''; }

        try {
            const formData = new FormData(form);

            pruneEmptyFields(formData, ['NewPassword', 'ConfirmPassword']);

            const fileInput = profilePicInput;
            if (fileInput && (!fileInput.files || fileInput.files.length === 0 || (fileInput.files[0] && fileInput.files[0].size === 0))) {
                formData.delete('ProfilePicFile');
            }

            for (const pair of formData.entries()) {
                if (pair[1] instanceof File) {
                    console.log('FormData:', pair[0], 'File{name:', pair[1].name, 'size:', pair[1].size + '}');
                } else {
                    console.log('FormData:', pair[0], pair[1]);
                }
            }

            const token = localStorage.getItem('jwt_token');
            const actionUrl = form.getAttribute('action') || '/api/users/edit';
            const headers = {};

            const anti = getAntiForgeryToken();
            if (anti) headers['RequestVerification-token'] = anti;

            if (token) headers['Authorization'] = `Bearer ${token}`;

            const res = await fetch(actionUrl, {
                method: 'PUT', 
                body: formData,
                headers
            });

            console.log('fetch done, status=', res.status);

            if (res.ok) {
                const data = await res.json();

                const newToken = data.token || data.Token;
                if (newToken) {
                    console.log('Novo token recebido. Atualizando localStorage...');
                    localStorage.setItem('jwt_token', newToken);
                }

                if (typeof Swal !== 'undefined') {
                    Swal.fire({ icon: 'success', title: 'Perfil atualizado!', timer: 1200, showConfirmButton: false })
                        .then(() => window.location.href = '/profile');
                } else {
                    alert('Perfil atualizado!');
                    window.location.href = '/profile';
                }
                return;
            }

            let errText = `Status ${res.status}`;
            try {
                const ct = res.headers.get('content-type') || '';
                if (ct.includes('application/json')) {
                    const json = await res.json();
                    if (json.errors) {
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