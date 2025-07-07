function decodeJwtPayload(token) {
    try {
        const base64Payload = token.split('.')[1];
        const payload = JSON.parse(atob(base64Payload));
        return payload;
    } catch (e) {
        console.error("Erro ao decodificar token:", e);
        return null;
    }
}

function checkAuthentication() {
    const token = localStorage.getItem('jwt_token');
    const userArea = document.getElementById('userArea');

    if (!token || !userArea) return;

    const payload = decodeJwtPayload(token);

    if (payload && payload.name) {
        userArea.innerHTML = `
            <li class="nav-item dropdown">
                <a class="nav-link dropdown-toggle" href="#" data-bs-toggle="dropdown">
                    <i class="bi bi-person-circle me-2"></i>
                    Olá, ${payload.name}
                </a>
                <ul class="dropdown-menu dropdown-menu-end">
                    <li><a class="dropdown-item" href="#">Meu Perfil</a></li>
                    <li><hr class="dropdown-divider" /></li>
                    <li><a class="dropdown-item" href="#" onclick="logout()">Sair</a></li>
                </ul>
            </li>
        `;
    }
}

function logout() {
    localStorage.removeItem('jwt_token');
    localStorage.removeItem('user_name');
    window.location.href = '/';

document.addEventListener('DOMContentLoaded', checkAuthentication);

document.addEventListener('DOMContentLoaded', () => {
    const userArea = document.getElementById('userArea');
    const userName = localStorage.getItem('user_name');

    if (userName && userArea) {
        userArea.innerHTML = `
            <li class="nav-item dropdown">
                <a class="nav-link dropdown-toggle" href="#" data-bs-toggle="dropdown">
                    <i class="bi bi-person-circle me-2"></i>
                    Olá, ${userName}
                </a>
                <ul class="dropdown-menu dropdown-menu-end">
                    <li><a class="dropdown-item" href="#">Meu Perfil</a></li>
                    <li><hr class="dropdown-divider" /></li>
                    <li><a class="dropdown-item" href="#" onclick="logout()">Sair</a></li>
                </ul>
            </li>
        `;
    }
});
}


