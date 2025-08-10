document.addEventListener('DOMContentLoaded', function() {

    async function loadDashboardData() {
        try {
            const token = localStorage.getItem('jwt_token');

            if (!token) {
                console.error("Usuário não autenticado. Token não encontrado.");
                document.body.innerHTML = '<div class="alert alert-danger text-center">Acesso negado. Por favor, faça o login novamente.</div>';
                return;
            }

            const response = await fetch('/api/dashboard/stats', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`
                }
            });

            if (!response.ok) {
                throw new Error(`Erro ao buscar dados: ${response.status}`);
            }

            const data = await response.json();

            populateDashboard(data);

        } catch (error) {
            console.error("Erro ao carregar dados do dashboard:", error);
            document.body.innerHTML = '<div class="alert alert-danger text-center">Não foi possível carregar os dados do dashboard. Tente novamente mais tarde.</div>';
        }
    }

    function populateDashboard(data) {
        document.getElementById('barbershop-name').textContent = data.barberShopName;

        document.getElementById('appointments-today').textContent = data.stats.appointmentsToday;
        document.getElementById('revenue-today').textContent = `R$ ${data.stats.revenueToday.toFixed(2)}`;
        document.getElementById('new-clients-month').textContent = data.stats.newClientsMonth;
        document.getElementById('average-rating').textContent = data.stats.averageRating.toFixed(1);

        const appointmentsTableBody = document.getElementById('appointments-table-body');
        appointmentsTableBody.innerHTML = '';
        if (data.upcomingAppointments && data.upcomingAppointments.length > 0) {
            data.upcomingAppointments.forEach(apt => {
                const row = `
                    <tr>
                        <td>${apt.client}</td>
                        <td>${apt.service}</td>
                        <td>${apt.time}</td>
                        <td><span class="badge bg-success">${apt.status}</span></td>
                    </tr>
                `;
                appointmentsTableBody.innerHTML += row;
            });
        } else {
            appointmentsTableBody.innerHTML = '<tr><td colspan="4" class="text-center p-5">Nenhum agendamento para hoje.</td></tr>';
        }

        initializeChart(data.weeklyRevenue);
    }

    function initializeChart(revenueData) {
        const ctx = document.getElementById('revenue-chart').getContext('2d');
        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: ['Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sáb', 'Dom'],
                datasets: [{
                    label: 'Faturamento R$',
                    data: revenueData,
                    backgroundColor: 'rgba(197, 168, 106, 0.6)',
                    borderColor: 'rgba(197, 168, 106, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                scales: {
                    y: { beginAtZero: true }
                },
                plugins: { legend: { display: false } }
            }
        });
    }

    loadDashboardData();
});