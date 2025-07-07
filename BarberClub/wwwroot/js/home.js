document.addEventListener('DOMContentLoaded', function () {
    const token = localStorage.getItem('jwt_token');

    const clientView = document.getElementById('client-view');
    const barberDashboardView = document.getElementById('barber-dashboard-view');
    const welcomeMessageSpan = document.getElementById('welcome-message');

    if (token) {
        // Se o token existe, asumimos que o usuário está logado
        clientView.style.display = 'none';
        barberDashboardView.style.display = 'block';

        // Tenta decodificar o token para pegar o nome do usuário (opcional, mas recomendado)
        try {
            // A função atob decodifica uma string base64. O payload do JWT é a segunda parte.
            const payload = JSON.parse(atob(token.split('.')[1]));
            // O nome do usuário está no campo "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
            const username = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];

            if (username && welcomeMessageSpan) {
                welcomeMessageSpan.textContent = `Bem-vindo(a), ${username}!`;
            }
        } catch (e) {
            console.error('Não foi possível decodificar o token:', e);
            // Caso o token seja inválido, talvez redirecionar para o login
            // localStorage.removeItem('jwt_token');
            // window.location.href = '/Account/Login';
        }

    } else {
        // Se não há token, mostra a visão normal de cliente
        clientView.style.display = 'block';
        barberDashboardView.style.display = 'none';
    }
});