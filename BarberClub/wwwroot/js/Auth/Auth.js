// Em wwwroot/js/Auth/Auth.js

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
function checkAuthentication() {
    const userMenu = document.getElementById('user-menu');
    if (!userMenu) return;

    const token = localStorage.getItem('jwt_token');

    const currentPagePath = window.location.pathname.toLowerCase(); 
    const registerPath = "/barbershopApi/register";
    const dashboardPath = "/navbar/dashboard";

    let registerButtonHtml = '';

    if (token) {
        const payload = decodeJwtPayload(token);
        console.log("Conteúdo do Token (Payload):", payload);
        const userHasBarberShops = String(payload.hasBarberShops).toLowerCase() === 'true';

        if (userHasBarberShops && currentPagePath !== dashboardPath) {
            registerButtonHtml = `
            <li class="nav-item me-3">
                <a class="btn btn-gold" href="/NavBar/Dashboard">
                    <i class="bi bi-speedometer2"></i> Dashboard
                </a>
            </li>
        `;
        } else if (!userHasBarberShops && currentPagePath !== registerPath) {
            registerButtonHtml = `
            <li class="nav-item me-3">
                <a class="nav-link-gold" href="/barbershops/register">
                    Tem uma barbearia? <br> Cadastre-a aqui!
                </a>
            </li>
        `;
        }

        if (payload && payload.firstName) {
            userMenu.innerHTML = registerButtonHtml + `
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
    } else {
        if (currentPagePath !== registerPath) {
            registerButtonHtml = `
            <li class="nav-item me-3">
                <a class="nav-link-gold" href="/barbershop/register">
                    Tem uma barbearia? <br> Cadastre-a aqui!
                </a>
            </li>
        `;
        }

        userMenu.innerHTML = registerButtonHtml + `
            <li class="nav-item">
                <a class="btn btn-gold" href="/Auth/Login">
                    <i class="bi bi-box-arrow-in-right"></i> Entrar
                </a>
            </li>
        `;
    }
}

function logout() {
    localStorage.removeItem('jwt_token');
    window.location.href = '/';
}

document.addEventListener('DOMContentLoaded', checkAuthentication);