let allUsers = [];
let searchTerm = "";
let searchTimeout;
let currentPage = 1;


// 1. Session and Token control
const checkAuth = () => {
    const token = localStorage.getItem('accessToken');
    if (!token) {
        console.warn("Yetkisiz erişim: Login sayfasına yönlendiriliyor...");
        window.location.href = 'login.html';
        return null;
    }
    return token;
};

// 2. Updating dashboard statistics
const updateDashboardStats = (pagedData) => {
    if (!pagedData) {
        console.warn("Dashboard güncellenemedi: Veri henüz hazır değil.");
        return;
    }

    const elements = {
        total: document.getElementById('statTotalUsers'),
        active: document.getElementById('statActiveUsers'),
        admins: document.getElementById('statAdminCount')
    };

    const total = pagedData.totalCount ?? pagedData.TotalCount ?? 0;
    const active = pagedData.activeCount ?? pagedData.ActiveCount ?? 0;
    const admin = pagedData.adminCount ?? pagedData.AdminCount ?? 0;

    if (elements.total) elements.total.innerText = total;
    if (elements.active) elements.active.innerText = active;
    if (elements.admins) elements.admins.innerText = admin;
};

// 3. Rendering user table
const renderUserTable = (users) => {
    const tableBody = document.getElementById('userTableBody');
    if (!tableBody) return;

    if (users.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="5" class="text-center">Sistemde kullanıcı bulunamadı.</td></tr>';
        return;
    }

    tableBody.innerHTML = users.map(user => {
        const userId = user.id || user.Id;
        const isActive = user.isActive || user.IsActive;
        const username = user.username || user.Username || 'İsimsiz';
        const roles = user.roles?.length > 0 ? user.roles.join(', ') : 'Rol Atanmamış';

        return `
            <tr>
                <td>
                    <i class="ri-user-line ri-22px text-primary me-2"></i>
                    <strong>${username}</strong>
                </td>
                <td>${user.email || '-'}</td>
                <td><span class="badge bg-label-primary">${roles}</span></td>
                <td>
                    <span class="badge ${isActive ? 'bg-label-success' : 'bg-label-secondary'}">
                        ${isActive ? 'Active' : 'Suspended'}
                    </span>
                </td>
                <td>
                    <button class="btn btn-sm btn-outline-primary me-2" onclick="openEditModal(${userId})">
                        <i class="ri-edit-box-line"></i> Düzenle
                    </button>
                    <button class="btn btn-sm btn-danger" onclick="deleteUser(${userId})">
                        <i class="ri-delete-bin-line"></i> Sil
                    </button>
                </td>
            </tr>`;
    }).join('');
};

// 5. Searching
async function handleSearch(event) {
    searchTerm = event.target.value;
    currentPage = 1; // Arama yapılınca her zaman 1. sayfaya dönmeliyiz
    await fetchUsers(1);
}

//filtering
window.applyFilters = function () {
    currentPage = 1; 
    fetchUsers(1);
};

// Initialization
document.addEventListener('DOMContentLoaded', async () => {
    if (!checkAuth()) return;
    await fetchRoles();
    await fetchPermissions();
    const nameEl = document.getElementById('navUserName');
    const roleEl = document.getElementById('navUserRole');
    if (nameEl) nameEl.innerText = localStorage.getItem('username') || 'User';
    if (roleEl) roleEl.innerText = localStorage.getItem('userRole') || 'Guest';

    const searchInput = document.getElementById('userSearchInput');
    if (searchInput) {
        searchInput.addEventListener('input', (e) => {
            clearTimeout(searchTimeout);
            searchTerm = e.target.value;
            searchTimeout = setTimeout(() => {
                console.log("Arama yapılıyor:", searchTerm);
                fetchUsers(1); 
            }, 500);
        });
    }

    await fetchUsers(1);
});


//API Functions (CRUD)
async function deleteUser(id) {
    const result = await Swal.fire({
        title: 'Emin misiniz?',
        text: "Bu işlem geri alınamaz!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Evet, sil!',
        cancelButtonText: 'İptal'
    });

    if (result.isConfirmed) {
        try {
            await api.delete(`/users/${id}`);
            Swal.fire('Silindi!', 'Kullanıcı başarıyla silindi.', 'success');
            fetchUsers(currentPage); 
        } catch (error) {
            Swal.fire('Hata!', 'Silme işlemi başarısız.', 'error');
        }
    }
}

async function saveUser() {
    const userData = {
        Username: document.getElementById('newUserName').value,
        Email: document.getElementById('newUserEmail').value,
        Password: document.getElementById('newUserPassword').value,
        RoleIds: [parseInt(document.getElementById('newUserRole').value)]
    };

    if (!userData.Username || !userData.Password) {
        Swal.fire('Uyarı!', 'Lütfen kullanıcı adı ve şifre alanlarını doldurun.', 'warning');
        return;
    }

    try {
        await api.post('/users/create', userData);

        const modalElement = document.getElementById('addUserModal');
        bootstrap.Modal.getInstance(modalElement).hide();

        document.getElementById('addUserForm').reset();

        await Swal.fire({
            icon: 'success',
            title: 'Kayıt Başarılı!',
            text: `${userData.Username} sisteme eklendi.`,
            timer: 2000,
            showConfirmButton: false
        });

        fetchUsers(1);

    } catch (error) {
        const errorMessage = error.response?.data?.Message || "Kullanıcı eklenirken bir hata oluştu.";

        Swal.fire({
            icon: 'error',
            title: 'Kayıt Durduruldu!',
            text: errorMessage, 
            confirmButtonColor: '#696cff'
        });
    }
}

function openEditModal(userId) {
    const user = allUsers.find(u => (u.id || u.Id) === userId);
    if (!user) return;

    document.getElementById('editUserId').value = user.id || user.Id;
    document.getElementById('editUserName').value = user.username || user.Username;
    document.getElementById('editUserEmail').value = user.email || user.Email;
    document.getElementById('editIsActive').checked = user.isActive || user.IsActive;

    const roleSelect = document.getElementById('editUserRole');
    if (roleSelect) {
        const rId = user.roleId || user.RoleId;
        console.log("Düzenlenen Kullanıcı Rol ID:", rId);
        console.log("Select İçindeki Mevcut Opsiyonlar:", roleSelect.innerHTML);

        if (rId) {
            roleSelect.value = rId.toString();
        } 
    }

    const modalElement = document.getElementById('editUserModal');
    bootstrap.Modal.getOrCreateInstance(modalElement).show();
}

async function updateUser() {
    const id = document.getElementById('editUserId').value;
    const data = {
        Id: parseInt(id),
        Username: document.getElementById('editUserName').value,
        Email: document.getElementById('editUserEmail').value,
        IsActive: document.getElementById('editIsActive').checked,
        RoleIds: [parseInt(document.getElementById('editUserRole').value)]
    };

    try {
        await api.put(`/users/${id}`, data);

        const modalElement = document.getElementById('editUserModal');
        bootstrap.Modal.getInstance(modalElement).hide();

        const Toast = Swal.mixin({
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true
        });

        Toast.fire({
            icon: 'success',
            title: 'Kullanıcı güncellendi'
        });

        fetchUsers(currentPage); 

    } catch (error) {
        Swal.fire('Hata!', 'Güncelleme yapılamadı.', 'error');
    }
}

function logout() {
    Swal.fire({
        title: 'Çıkış Yapılıyor',
        text: "Oturumunuzu sonlandırmak istediğinize emin misiniz?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#696cff', // Temanın ana rengi
        cancelButtonColor: '#8592a3',
        confirmButtonText: 'Evet, çıkış yap',
        cancelButtonText: 'Vazgeç'
    }).then((result) => {
        if (result.isConfirmed) {
            // Tokenları temizle
            localStorage.clear();

            // Başarılı mesajı göster ve yönlendir
            Swal.fire({
                title: 'Hoşça kal!',
                text: 'Başarıyla çıkış yapıldı.',
                icon: 'success',
                timer: 1500,
                showConfirmButton: false
            }).then(() => {
                window.location.href = 'login.html';
            });
        }
    });
}

//page numbers
function renderPagination(data) {
    const paginationList = document.getElementById('paginationList');
    if (!paginationList) return;

    paginationList.innerHTML = "";


    for (let i = 1; i <= data.totalPages; i++) {
        const isActive = i === data.pageNumber ? 'active' : '';

        const li = `
            <li class="page-item ${isActive}">
                <a class="page-link" href="javascript:void(0);" onclick="fetchUsers(${i})">
                    ${i}
                </a>
            </li>`;

        paginationList.innerHTML += li;
    }
}



// fetching data
async function fetchUsers(page = 1) {
    currentPage = page;

    const status = document.getElementById('filterStatus').value;
    const roleId = document.getElementById('filterRole').value;


    try {
        const response = await api.get('/users', {
            params: {
                pageNumber: page,
                pageSize: 5,
                searchTerm: searchTerm,

                isActive: status === "" ? null : status,
                roleId: roleId === "" ? null : roleId
            }
        });

        const pagedData = response.data;
        allUsers = pagedData.items;

        renderUserTable(allUsers);

        renderPagination(pagedData);
        updateDashboardStats(pagedData);

    } catch (error) {
        console.error("Sayfa yükleme hatası:", error);
    }
}

//Handle exception
function handleApiError(error) {
    console.error("API Hatası:", error);
    if (error.response?.status === 401) {
        alert("Oturum süreniz dolmuş.");
        window.location.href = 'login.html';
    } else {
        alert("Sunucuya bağlanılamadı. API'nin çalıştığından emin olun.");
    }
}

async function fetchRoles() {
    try {
        const response = await api.get('Roles'); // baseURL: .../api/ olduğu için sadece 'Roles'
        const roles = response.data;

        // Sayfadaki 3 farklı select elementini yakalıyoruz
        const selects = {
            add: document.getElementById('newUserRole'),
            edit: document.getElementById('editUserRole'),
            filter: document.getElementById('filterRole')
        };

        // Her bir select'i dolduralım
        Object.keys(selects).forEach(key => {
            const select = selects[key];
            if (!select) return;

            // İçini temizle
            select.innerHTML = '';

            // Eğer filtreleme kutusuysa en başa "Tüm Roller" ekleyelim
            if (key === 'filter') {
                const defaultOpt = document.createElement('option');
                defaultOpt.value = "";
                defaultOpt.textContent = "Tüm Roller";
                select.appendChild(defaultOpt);
            }

            // Backend'den gelen her rolü ekle
            roles.forEach(role => {
                const option = document.createElement('option');
                option.value = role.id || role.Id; // Backend'den gelen 'id'
                option.textContent = role.name || role.Name; // Backend'den gelen 'name'
                select.appendChild(option);
            });
        });

        console.log("Roller dinamik olarak yüklendi!");
    } catch (error) {
        console.error("Roller yüklenirken bir hata oluştu:", error);
    }
}

async function fetchPermissions() {
    const container = document.getElementById('permissionsChecklist');
    try {
        const response = await api.get('Permissions'); // Backend'de GetPermissions endpoint'i olmalı
        const permissions = response.data;

        container.innerHTML = permissions.map(p => `
            <div class="form-check mb-2">
                <input class="form-check-input perm-checkbox" type="checkbox" value="${p.id}" id="perm_${p.id}">
                <label class="form-check-label" for="perm_${p.id}">
                    ${p.name} <small class="text-muted">(${p.description || ''})</small>
                </label>
            </div>
        `).join('');
    } catch (e) {
        container.innerHTML = '<span class="text-danger">Yetkiler yüklenemedi.</span>';
    }
}

async function fetchPermissions() {
    const container = document.getElementById('permissionsChecklist');
    try {
        const response = await api.get('Permissions'); // Backend'de GetPermissions endpoint'i olmalı
        const permissions = response.data;

        container.innerHTML = permissions.map(p => `
    <div class="form-check mb-2">
        <input class="form-check-input perm-checkbox" type="checkbox" value="${p.id}" id="perm_${p.id}">
        <label class="form-check-label" for="perm_${p.id}">
            <strong>${p.key}</strong> - <small class="text-muted">${p.description}</small>
        </label>
    </div>
`).join('');
    } catch (e) {
        container.innerHTML = '<span class="text-danger">Yetkiler yüklenemedi.</span>';
    }
}
