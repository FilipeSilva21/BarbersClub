document.addEventListener('DOMContentLoaded', function() {
    // Função principal para carregar todos os dados do dashboard
    async function loadDashboardData() {
        // Simulação de chamada de API. Substitua pela sua chamada real.
        // Você precisará criar os endpoints no seu back-end.
        try {
            // Exemplo de como você buscaria os dados
            // const response = await fetch('/api/dashboard/stats');
            // const data = await response.json();

            // Usando dados de exemplo por enquanto
            const mockData = {
                barberShopName: "Navalha de Ouro",
                stats: {
                    appointmentsToday: 8,
                    revenueToday: 320.00,
                    newClientsMonth: 15,
                    averageRating: 4.9
                },
                upcomingAppointments: [
                    { client: "Carlos Pereira", service: "Corte + Barba", time: "14:30", status: "Confirmado" },
                    { client: "Lucas Almeida", service: "Corte Degradê", time: "15:00", status: "Confirmado" },
                    { client: "Mariana Costa", service: "Corte Feminino", time: "16:00", status: "Aguardando" }
                ],
                weeklyRevenue: [150, 220, 300, 250, 400, 500, 350]
            };

            populateDashboard(mockData);

        } catch (error) {
            console.error("Erro ao carregar dados do dashboard:", error);
            // Mostrar mensagem de erro na tela
        }
    }

    function populateDashboard(data) {
        // Preenche o nome da barbearia
        document.getElementById('barbershop-name').textContent = data.barberShopName;

        // Preenche os cartões de estatísticas
        document.getElementById('appointments-today').textContent = data.stats.appointmentsToday;
        document.getElementById('revenue-today').textContent = `R$ ${data.stats.revenueToday.toFixed(2)}`;
        document.getElementById('new-clients-month').textContent = data.stats.newClientsMonth;
        document.getElementById('average-rating').textContent = data.stats.averageRating.toFixed(1);

        // Preenche a tabela de agendamentos
        const appointmentsTableBody = document.getElementById('appointments-table-body');
        appointmentsTableBody.innerHTML = ''; // Limpa a mensagem de "carregando"
        if (data.upcomingAppointments.length > 0) {
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

        // Inicializa o gráfico
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

    // Carrega os dados quando a página estiver pronta
    loadDashboardData();
});