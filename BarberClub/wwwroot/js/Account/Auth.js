function decodeJwtPayload(token) {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = JSON.parse(atob(base64));
        return jsonPayload;
    } catch (e) {
        console.error("Erro ao decodificar token:", e);
        return null;
    }
}

function checkAuthentication() {
    const token = localStorage.getItem('jwt_token');
    const userMenu = document.getElementById('user-menu');

    if (!token || !userMenu) {
        return; 
    }

    const payload = decodeJwtPayload(token);

    if (payload && payload.firstName) {
        userMenu.innerHTML = `
            <li class="nav-item dropdown">
                <a class="nav-link dropdown-toggle d-flex align-items-center" href="#" id="navbarDropdown" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                    <i class="bi bi-person-circle me-2" style="font-size: 1.2rem;"></i>
                    Olá, ${payload.firstName}
                </a>
                <ul class="dropdown-menu dropdown-menu-dark dropdown-menu-end" aria-labelledby="navbarDropdown">
                    <li><a class="dropdown-item" href="/Account/Profile">Meu Perfil</a></li>
                    <li><hr class="dropdown-divider"></li>
                    <li>
                        <button type="button" class="dropdown-item" onclick="logout()">
                            <i class="bi bi-box-arrow-right me-2"></i>Sair
                        </button>
                    </li>
                </ul>
            </li>
    `;
    }
}

function logout() {
    localStorage.removeItem('jwt_token');
    window.location.href = '/';
}

document.addEventListener('DOMContentLoaded', checkAuthentication);