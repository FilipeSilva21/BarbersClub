// Em wwwroot/js/Auth/Auth.js

/**
 * Decodifica o payload de um token JWT.
 */
function decodeJwtPayload(token) {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = JSON.parse(atob(base64));
        jsonPayload.firstName = jsonPayload.firstName || jsonPayload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];
        return jsonPayload;
    } catch (e) {
        console.error("Erro ao decodificar token:", e);
        return null;
    }
}

/**
 * Gera o HTML para o link de cadastro de barbearia, se necessário.
 */
function getRegistrationMarkup() {
    const currentPagePath = window.location.pathname.toLowerCase();
    const registerPagePath = "/barbershops/register"; // Rota correta da PÁGINA

    if (currentPagePath !== registerPagePath) {
        return `
            <li class="nav-item me-3">
                <a class="nav-link-gold nav-link-jwt" href="/barbershops/register"> Tem uma barbearia? <br> Cadastre-a aqui!
                </a>
            </li>
        `;
    }
    return '';
}

/**
 * Renderiza o menu para um usuário autenticado.
 */
function renderAuthenticatedMenu(payload) {
    const userMenu = document.getElementById('user-menu');
    const hasBarberShop = String(payload.hasBarbershops).toLowerCase() === 'true';
    const currentPagePath = window.location.pathname.toLowerCase();
    const dashboardPagePath = "/navbar/dashboard";

    let dynamicButtonHtml = '';

    if (hasBarberShop) {
        if (currentPagePath !== dashboardPagePath) {
            dynamicButtonHtml = `
                <li class="nav-item me-3">
                    <a class="btn btn-gold nav-link-jwt" href="/navbar/dashboard"> <i class="bi bi-speedometer2"></i> Dashboard
                    </a>
                </li>
            `;
        }
    } else {
        dynamicButtonHtml = getRegistrationMarkup();
    }

    userMenu.innerHTML = dynamicButtonHtml + `
        <li class="nav-item dropdown">
            <a class="nav-link dropdown-toggle d-flex align-items-center" href="#" id="navbarDropdown" role="button" data-bs-toggle="dropdown" aria-expanded="false">
                <i class="bi bi-person-circle me-2" style="font-size: 1.2rem;"></i>
                Olá, ${payload.firstName}
            </a>
            <ul class="dropdown-menu dropdown-menu-dark dropdown-menu-end" aria-labelledby="navbarDropdown">
                <li><a class="dropdown-item nav-link-jwt" href="/profile">Meu Perfil</a></li> <li><hr class="dropdown-divider"></li>
                <li>
                    <button type="button" class="dropdown-item" onclick="logout()">
                        <i class="bi bi-box-arrow-right me-2"></i>Sair
                    </button>
                </li>
            </ul>
        </li>
    `;
}

/**
 * Renderiza o menu para um visitante (convidado).
 */
function renderGuestMenu() {
    const userMenu = document.getElementById('user-menu');
    const registrationMarkup = getRegistrationMarkup();

    userMenu.innerHTML = registrationMarkup + `
        <li class="nav-item">
            <a class="btn btn-gold" href="/Auth/Login"> <i class="bi bi-box-arrow-in-right"></i> Entrar
            </a>
        </li>
    `;
}

/**
 * Função principal que verifica a autenticação e atualiza a UI.
 */
function checkAuthentication() {
    const userMenu = document.getElementById('user-menu');
    if (!userMenu) return;

    const token = localStorage.getItem('jwt_token');

    if (token) {
        const payload = decodeJwtPayload(token);
        if (payload) {
            renderAuthenticatedMenu(payload);
        } else {
            logout(); // Se o token for inválido, limpa e trata como deslogado
        }
    } else {
        renderGuestMenu();
    }
}

/**
 * Realiza o logout do usuário.
 */
async function logout() {
    localStorage.removeItem('jwt_token');

    await fetch('/api/auth/logout', { method: 'POST' }); 

    window.location.href = '/';
}

document.addEventListener('DOMContentLoaded', checkAuthentication);