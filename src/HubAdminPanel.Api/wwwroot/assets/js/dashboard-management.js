
// 2. Updating dashboard statistics
let statusChart, roleChart;
function updateDashboardStats(pagedData) {
    const total = pagedData.totalCount || 0;
    const active = pagedData.activeCount || 0;
    const admin = pagedData.adminCount || 0;
    const suspended = total - active;

    if (document.getElementById('statTotalUsers')) document.getElementById('statTotalUsers').innerText = total;
    if (document.getElementById('statActiveUsers')) document.getElementById('statActiveUsers').innerText = active;
    if (document.getElementById('statAdminCount')) document.getElementById('statAdminCount').innerText = admin;

    const ctxStatus = document.getElementById('statusChart').getContext('2d');
    if (statusChart) statusChart.destroy();
    statusChart = new Chart(ctxStatus, {
        type: 'doughnut',
        data: {
            labels: ['Aktif', 'Suspended'],
            datasets: [{
                data: [active, suspended],
                backgroundColor: ['#72e128', '#8592a3'],
                hoverOffset: 4
            }]
        },
        options: { responsive: true, maintainAspectRatio: false }
    });

    const roleCounts = {};
    pagedData.items.forEach(u => {
        const roleName = u.roles[0] || 'Atanmamış';
        roleCounts[roleName] = (roleCounts[roleName] || 0) + 1;
    });

    const roleData = pagedData.roleCounts || {};

    const ctxRole = document.getElementById('roleChart').getContext('2d');
    if (roleChart) roleChart.destroy();

    roleChart = new Chart(ctxRole, {
        type: 'bar',
        data: {
            labels: Object.keys(roleData),
            datasets: [{
                label: 'Sistemdeki Toplam Kullanıcı',
                data: Object.values(roleData),
                backgroundColor: '#666cff',
                borderRadius: 5
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: { stepSize: 1 }
                }
            }
        }
    });
}