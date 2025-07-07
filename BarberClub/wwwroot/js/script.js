// Conteúdo de wwwroot/js/script.js

document.addEventListener('DOMContentLoaded', function() {
    // Lógica para o formulário de logout, que está no layout e aparece em todas as páginas
    const logoutForm = document.getElementById('logoutForm');

    if (logoutForm) {
        logoutForm.addEventListener('submit', function() {
            // Remove o token do localStorage ao clicar em "Sair"
            localStorage.removeItem('jwt_token');
        });
    }
});