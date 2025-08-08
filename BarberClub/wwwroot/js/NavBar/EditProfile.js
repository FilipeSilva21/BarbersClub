// wwwroot/js/navbar/EditProfile.js

document.addEventListener('DOMContentLoaded', function () {
    const imageUpload = document.getElementById('ProfilePicFile');
    const imagePreview = document.getElementById('image-preview');

    if (imageUpload) {
        // Lógica de pré-visualização da imagem de perfil
        imageUpload.addEventListener('change', function (event) {
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
});