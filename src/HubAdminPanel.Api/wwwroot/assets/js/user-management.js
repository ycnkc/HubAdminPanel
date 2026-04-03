let allUsers = [];
let searchTerm = "";
let searchTimeout;
let currentPage = 1;

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
                    <button class="btn btn-sm btn-outline-primary me-2 btn-perm" 
                            data-roles="Admin" 
                            onclick="openEditModal(${userId})">
                        <i class="ri-edit-box-line"></i> Düzenle
                    </button>
                    
                    <button class="btn btn-sm btn-danger btn-perm" 
                            data-roles="Admin" 
                            onclick="deleteUser(${userId})">
                        <i class="ri-delete-bin-line"></i> Sil
                    </button>
                </td>
            </tr>`;
    }).join('');

    if (typeof checkUIByRoles === 'function') {
        checkUIByRoles();
    }
};

async function handleSearch(event) {
    searchTerm = event.target.value;
    currentPage = 1;
    await fetchUsers(1);
}

window.applyFilters = function () {
    currentPage = 1;
    fetchUsers(1);
};

document.addEventListener('DOMContentLoaded', async () => {
    if (!checkAuth()) return;
    await fetchRoles();

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

async function fetchUsers(page = 1) {
    const status = document.getElementById('filterStatus').value;
    const roleId = document.getElementById('filterRole').value;

    try {
        const response = await api.get('/users', {
            params: {
                pageNumber: page,
                pageSize: PaginationManager.pageSize,
                searchTerm: typeof searchTerm !== 'undefined' ? searchTerm : '',
                isActive: status === "" ? null : status,
                roleId: roleId === "" ? null : roleId
            }
        });

        const pagedData = response.data;
        allUsers = pagedData.items;

        renderUserTable(allUsers);

        PaginationManager.update(pagedData, fetchUsers);

        if (typeof updateDashboardStats === 'function') {
            updateDashboardStats(pagedData);
        }

        if (typeof checkUIByRoles === 'function') {
            checkUIByRoles();
        }

    } catch (error) {
        console.error("Sayfa yükleme hatası:", error);
    }
}

function handleApiError(error) {
    console.error("API Hatası:", error);
    if (error.response?.status === 401) {
        alert("Oturum süreniz dolmuş.");
        window.location.href = 'login.html';
    } else {
        alert("Sunucuya bağlanılamadı. API'nin çalıştığından emin olun.");
    }
}

let modalEndpoints = []; 
async function fetchEndpointsForModal() {
    try {
        const response = await api.get('/Management/endpoints');
        modalEndpoints = response.data;
        renderEndpointChecklist(modalEndpoints);
    } catch (error) {
        document.getElementById('endpointChecklist').innerHTML = '<div class="text-danger">Yüklenemedi.</div>';
    }
}

function renderEndpointChecklist(endpoints) {
    const container = document.getElementById('endpointChecklist');
    container.innerHTML = '';

    if (endpoints.length === 0) {
        container.innerHTML = '<div class="text-muted small">Eşleşen endpoint bulunamadı.</div>';
        return;
    }

    endpoints.forEach(ep => {
        const methods = { 'GET': 'text-success', 'POST': 'text-primary', 'PUT': 'text-warning', 'DELETE': 'text-danger' };
        const methodClass = methods[ep.method] || 'text-secondary';

        const item = `
            <div class="form-check endpoint-item mb-1" data-path="${ep.path.toLowerCase()}" data-method="${ep.method}">
                <input class="form-check-input endpoint-checkbox" type="checkbox" value="${ep.id}" id="ep_${ep.id}">
                <label class="form-check-label d-flex justify-content-between w-100" for="ep_${ep.id}">
                    <span class="small fw-bold ${methodClass} me-2">${ep.method}</span>
                    <span class="small text-truncate">${ep.path}</span>
                </label>
            </div>`;
        container.insertAdjacentHTML('beforeend', item);
    });
}

function filterEndpointsInModal() {
    const searchQuery = document.getElementById('endpointSearchInModal').value.toLowerCase();
    const selectedMethod = document.getElementById('methodFilterInModal').value;

    document.querySelectorAll('.endpoint-item').forEach(el => {
        const path = el.getAttribute('data-path');
        const method = el.getAttribute('data-method');

        const matchesSearch = path.includes(searchQuery);
        const matchesMethod = selectedMethod === "" || method === selectedMethod;

        if (matchesSearch && matchesMethod) {
            el.classList.remove('d-none');
        } else {
            el.classList.add('d-none');
        }
    });
}

async function createNewRole() {
    const roleName = document.getElementById('newRoleName').value.trim();
    const selectedEndpoints = Array.from(document.querySelectorAll('.endpoint-checkbox:checked'))
        .map(cb => parseInt(cb.value));

    if (!roleName) return Swal.fire('Hata', 'Lütfen rol adı girin', 'error');

    try {
        await api.post('/Roles/create', {
            Name: roleName,
            EndpointIds: selectedEndpoints
        });

        Swal.fire('Başarılı', 'Rol ve endpoint yetkileri oluşturuldu', 'success');
        bootstrap.Modal.getInstance(document.getElementById('addRoleModal')).hide();
        location.reload();
    } catch (error) {
        Swal.fire('Hata', 'İşlem başarısız oldu', 'error');
    }
}

function toggleSelectAll(mainCheckbox) {
    const checkboxes = document.querySelectorAll('.endpoint-checkbox');
    
    checkboxes.forEach(cb => {
        const parentItem = cb.closest('.endpoint-item');
        if (parentItem && !parentItem.classList.contains('d-none')) {
            cb.checked = mainCheckbox.checked;
        }
    });
}

document.addEventListener('DOMContentLoaded', () => {
    fetchEndpointsForModal();
});