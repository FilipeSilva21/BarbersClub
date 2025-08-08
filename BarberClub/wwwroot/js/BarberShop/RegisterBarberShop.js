// Em wwwroot/js/BarberShop/RegisterBarberShop.js

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
});