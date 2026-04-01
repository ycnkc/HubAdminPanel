document.addEventListener('DOMContentLoaded', function () {
    loadEndpoints();
    loadPermissions(); 
});

async function loadEndpoints() {
    try {
        const response = await axios.get('/api/Management/endpoints');
        const endpoints = response.data;
        const tableBody = document.getElementById('endpointTableBody');
        tableBody.innerHTML = '';

        endpoints.forEach(ep => {
            const methodBadge = getMethodBadge(ep.method);

            const permissionsHtml = ep.endpointPermissionMappings && ep.endpointPermissionMappings.length > 0
                ? ep.endpointPermissionMappings.map(m => `
                    <span class="badge bg-label-info me-1">
                        ${m.permission.permissionKey} 
                        <i class="mdi mdi-close-circle ms-1 text-danger cursor-pointer" 
                           onclick="removePermission(${ep.id}, ${m.permissionId})"></i>
                    </span>`).join('')
                : '<span class="text-muted small">Yetki atanmamış</span>';

            tableBody.innerHTML += `
    <tr>
        <td>${methodBadge}</td>
        <td><span class="fw-medium">${ep.path}</span></td>
        <td><small>${ep.description || 'Açıklama yok'}</small></td>
        <td>${permissionsHtml}</td>
        <td>
            <div class="dropdown">
                <button type="button" class="btn p-0 dropdown-toggle hide-arrow" data-bs-toggle="dropdown">
                    <i class="mdi mdi-dots-vertical"></i>
                </button>
                <div class="dropdown-menu">
                    <a class="dropdown-item" href="javascript:void(0);" onclick="openAssignModal(${ep.id}, '${ep.path}')">
                        <i class="mdi mdi-shield-check-outline me-1"></i> Yetkiye Ata
                    </a>
                    <a class="dropdown-item" href="javascript:void(0);" onclick="openUserAssignModal(${ep.id}, '${ep.path}')">
                        <i class="mdi mdi-account-key-outline me-1"></i> Kullanıcıya Ata
                    </a>
                </div>
            </div>
        </td>
    </tr>
`;
        });
    } catch (error) {
        showToast('Hata', 'Endpointler yüklenirken bir sorun oluştu', 'error');
    }
}

async function loadPermissions() {
    try {
        const response = await axios.get('/api/Permissions'); 
        const permissions = response.data;
        const select = document.getElementById('permissionSelect');

        select.innerHTML = '<option value="">Yetki Seçiniz...</option>';
        permissions.forEach(p => {
            select.innerHTML += `<option value="${p.id}">${p.key}</option>`;
        });
    } catch (error) {
        console.error("Yetkiler yüklenemedi", error);
    }
}

function openAssignModal(id, path) {
    document.getElementById('selectedEndpointId').value = id;
    document.getElementById('endpointPathDisplay').value = path;

    const modal = new bootstrap.Modal(document.getElementById('assignPermissionModal'));
    modal.show();
}

async function savePermissionAssignment() {
    const endpointId = document.getElementById('selectedEndpointId').value;
    const permissionId = document.getElementById('permissionSelect').value;

    if (!permissionId) {
        showToast('Uyarı', 'Lütfen bir yetki seçin', 'warning');
        return;
    }

    try {
        await axios.post(`/api/Management/assign-permission?endpointId=${endpointId}&permissionId=${permissionId}`);

        const modalElement = document.getElementById('assignPermissionModal');
        const modal = bootstrap.Modal.getInstance(modalElement);
        modal.hide();

        showToast('Başarılı', 'Yetki başarıyla atandı', 'success');
        loadEndpoints(); 
    } catch (error) {
        showToast('Hata', error.response?.data || 'İşlem başarısız', 'error');
    }
}

async function removePermission(endpointId, permissionId) {
    Swal.fire({
        title: 'Emin misiniz?',
        text: "Bu yetki bu endpoint'ten kaldırılacak!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Evet, kaldır',
        cancelButtonText: 'Vazgeç'
    }).then(async (result) => {
        if (result.isConfirmed) {
            try {
                await axios.delete(`/api/Management/remove-permission?endpointId=${endpointId}&permissionId=${permissionId}`);
                showToast('Silindi', 'Yetki kaldırıldı', 'success');
                loadEndpoints();
            } catch (error) {
                showToast('Hata', 'Yetki kaldırılırken hata oluştu', 'error');
            }
        }
    });
}

function getMethodBadge(method) {
    const map = {
        'GET': 'bg-label-primary',
        'POST': 'bg-label-success',
        'PUT': 'bg-label-warning',
        'DELETE': 'bg-label-danger'
    };
    return `<span class="badge ${map[method] || 'bg-label-secondary'}">${method}</span>`;
}

function showToast(title, text, icon) {
    Swal.fire({ title, text, icon, toast: true, position: 'top-end', timer: 3000, showConfirmButton: false });
}

let allEndpoints = [];
let allUsers = [];

async function loadEndpoints() {
    try {
        const response = await api.get('/Management/endpoints');
        allEndpoints = response.data;
        renderTable(allEndpoints);
    } catch (error) {
        console.error(error);
    }
}

function renderTable(endpoints) {
    const tableBody = document.getElementById('endpointTableBody');
    if (!tableBody) return;
    tableBody.innerHTML = '';

    endpoints.forEach(ep => {
        const methods = { 'GET': 'bg-label-success', 'POST': 'bg-label-primary', 'PUT': 'bg-label-warning', 'DELETE': 'bg-label-danger' };
        const methodBadge = `<span class="badge ${methods[ep.method] || 'bg-label-secondary'}">${ep.method}</span>`;

        let permissionsHtml = '';
        const permCount = ep.endpointPermissionMappings?.length || 0;
        if (permCount > 0) {
            const firstPerm = ep.endpointPermissionMappings[0].permission?.key ||
                ep.endpointPermissionMappings[0].permission?.key || '???';

            if (permCount === 1) {
                permissionsHtml = `<div><span class="badge bg-label-info">${firstPerm}</span></div>`;
            } else {
                permissionsHtml = `
                    <div>
                        <span class="badge bg-label-info">${firstPerm}</span>
                        <a href="javascript:void(0);" onclick="showAllPermissions(${ep.id})" class="small text-info fw-bold ms-1 text-decoration-underline">
                            ve ${permCount - 1} yetki daha
                        </a>
                    </div>`;
            }
        }

        let usersHtml = '';
        const userCount = ep.endpointUsers?.length || 0;
        if (userCount > 0) {
            const firstUser = ep.endpointUsers[0].user?.username || ep.endpointUsers[0].user?.UserName || 'User';

            if (userCount === 1) {
                usersHtml = `<div class="mt-1"><span class="badge bg-label-success"><i class="mdi mdi-account-check me-1"></i>${firstUser}</span></div>`;
            } else {
                usersHtml = `
                    <div class="mt-1">
                        <span class="badge bg-label-success"><i class="mdi mdi-account-check me-1"></i>${firstUser}</span>
                        <a href="javascript:void(0);" onclick="showAllUsers(${ep.id})" class="small text-success fw-bold ms-1 text-decoration-underline">
                            ve ${userCount - 1} kişi daha
                        </a>
                    </div>`;
            }
        }

        const finalAccessHtml = (permissionsHtml + usersHtml) || '<span class="text-muted small italic">Atama yok</span>';

        tableBody.innerHTML += `
            <tr>
                <td>${methodBadge}</td>
                <td><span class="fw-medium">${ep.path}</span></td>
                <td><small class="text-truncate d-block" style="max-width: 150px;">${ep.description || ''}</small></td>
                <td>${finalAccessHtml}</td>
                <td>
                    <div class="dropdown">
                        <button type="button" class="btn p-0 dropdown-toggle hide-arrow" data-bs-toggle="dropdown">
                            <i class="mdi mdi-dots-vertical"></i>
                        </button>
                        <div class="dropdown-menu">
                            <a class="dropdown-item" href="javascript:void(0);" onclick="openAssignModal(${ep.id}, '${ep.path}')">
                                <i class="mdi mdi-shield-check-outline me-1"></i> Yetkiye Ata
                            </a>
                            <a class="dropdown-item" href="javascript:void(0);" onclick="openUserAssignModal(${ep.id}, '${ep.path}')">
                                <i class="mdi mdi-account-key-outline me-1"></i> Kullanıcıya Ata
                            </a>
                        </div>
                    </div>
                </td>
            </tr>`;
    });
}

async function openUserAssignModal(endpointId, path) {
    document.getElementById('assignEndpointId').value = endpointId;
    document.getElementById('selectedEndpointPath').innerText = path;
    document.getElementById('userSearchInput').value = '';
    document.getElementById('selectedUserId').value = '';
    document.getElementById('selectedUserBadge').style.setProperty('display', 'none', 'important');
    document.getElementById('userSearchResults').style.display = 'none';
    document.getElementById('btnAssignUser').disabled = true;

    const modalElement = document.getElementById('userAssignModal');
    const modal = new bootstrap.Modal(modalElement);
    modal.show();
}

let searchTimeout;

function searchUsers(query) {
    const resultsDiv = document.getElementById('userSearchResults');

    if (!query || query.trim().length < 2) {
        resultsDiv.style.display = 'none';
        return;
    }

    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(async () => {
        try {
            const res = await api.get(`/Management/search?query=${query}`);
            const filtered = Array.isArray(res.data) ? res.data : (res.data.data || []);

            resultsDiv.innerHTML = '';

            if (filtered.length > 0) {
                filtered.forEach(u => {
                    const name = u.username || u.userName;
                    const item = document.createElement('a');
                    item.href = "javascript:void(0);";
                    item.className = "list-group-item list-group-item-action py-2";
                    item.innerHTML = `<i class="mdi mdi-account-outline me-2"></i>${name}`;
                    item.onclick = () => selectUser(u.id, name);
                    resultsDiv.appendChild(item);
                });
                resultsDiv.style.display = 'block';
            } else {
                resultsDiv.innerHTML = '<div class="list-group-item text-muted small">Eşleşen kullanıcı yok.</div>';
                resultsDiv.style.display = 'block';
            }
        } catch (error) {
            console.error("Arama hatası:", error);
        }
    }, 300);
}

function selectUser(id, username) {
    document.getElementById('selectedUserId').value = id;
    document.getElementById('selectedUsernameText').innerText = username;
    document.getElementById('selectedUserBadge').style.setProperty('display', 'block', 'important');
    document.getElementById('userSearchResults').style.display = 'none';
    document.getElementById('userSearchInput').value = '';
    document.getElementById('btnAssignUser').disabled = false;
}

async function saveUserExtraPermission() {
    const endpointId = parseInt(document.getElementById('assignEndpointId').value);
    const userId = parseInt(document.getElementById('selectedUserId').value);

    if (!userId) {
        Swal.fire({
            icon: 'warning',
            title: 'Uyarı',
            text: 'Lütfen önce bir kullanıcı seçin!',
            confirmButtonColor: '#3085d6'
        });
        return;
    }

    try {
        await api.post('/Management/assign-user-to-endpoint', {
            endpointId: endpointId,
            userId: userId
        });

        await Swal.fire({
            icon: 'success',
            title: 'Başarılı!',
            text: 'Erişim izni başarıyla tanımlandı.',
            showConfirmButton: false,
            timer: 1500
        });

        location.reload();
    } catch (err) {
        Swal.fire({
            icon: 'error',
            title: 'Hata!',
            text: err.response?.data?.message || "İşlem sırasında bir hata oluştu.",
            confirmButtonColor: '#d33'
        });
    }
}

function showAllUsers(endpointId) {
    const searchInput = document.getElementById('modalListSearch');
    if (searchInput) searchInput.value = '';
    const ep = allEndpoints.find(e => e.id === endpointId);
    const listElement = document.getElementById('allUsersList');
    document.querySelector('#allUsersModal .modal-title').innerText = "Erişim İzni Olanlar";
    listElement.innerHTML = '';

    if (!ep.endpointUsers || ep.endpointUsers.length === 0) {
        listElement.innerHTML = '<li class="list-group-item text-muted small text-center">Özel erişim tanımlanmış kullanıcı yok.</li>';
    } else {
        ep.endpointUsers.forEach(u => {
            const name = u.user?.username || u.user?.UserName || 'User';
            listElement.innerHTML += `
                <li class="list-group-item d-flex justify-content-between align-items-center">
                    <span><i class="mdi mdi-account-outline me-2 text-success"></i>${name}</span>
                    <button class="btn btn-sm btn-outline-danger border-0" onclick="deleteUserAccess(${endpointId}, ${u.userId})">
                        <i class="mdi mdi-delete-outline"></i>
                    </button>
                </li>`;
        });
    }
    new bootstrap.Modal(document.getElementById('allUsersModal')).show();
}

async function deleteUserAccess(endpointId, userId) {
    const result = await Swal.fire({
        title: 'Emin misiniz?',
        text: "Kullanıcının bu endpoint'e özel erişimi kaldırılacak!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Evet, sil!',
        cancelButtonText: 'Vazgeç'
    });

    if (result.isConfirmed) {
        try {
            await api.delete(`/Management/remove-user-from-endpoint/${endpointId}/${userId}`);
            Swal.fire({
                icon: 'success',
                title: 'Silindi!',
                text: 'Kullanıcı erişimi başarıyla kaldırıldı.',
                timer: 1500,
                showConfirmButton: false
            });
            setTimeout(() => location.reload(), 1500);
        } catch (err) {
            Swal.fire('Hata!', err.response?.data?.message || 'Silme işlemi başarısız.', 'error');
        }
    }
}

function showAllPermissions(endpointId) {
    const searchInput = document.getElementById('modalListSearch');
    if (searchInput) searchInput.value = '';
    const ep = allEndpoints.find(e => e.id === endpointId);
    const listElement = document.getElementById('allUsersList');
    document.querySelector('#allUsersModal .modal-title').innerText = "Gerekli Grup Yetkileri";
    listElement.innerHTML = '';

    if (!ep.endpointPermissionMappings || ep.endpointPermissionMappings.length === 0) {
        listElement.innerHTML = '<li class="list-group-item text-muted small text-center">Herhangi bir yetki grubu atanmamış.</li>';
    } else {
        ep.endpointPermissionMappings.forEach(m => {
            const key = m.permission?.key|| '???';
            listElement.innerHTML += `
                <li class="list-group-item d-flex justify-content-between align-items-center">
                    <span><i class="mdi mdi-shield-outline me-2 text-info"></i>${key}</span>
                    <button class="btn btn-sm btn-outline-danger border-0" onclick="deletePermissionAccess(${endpointId}, ${m.permissionId})">
                        <i class="mdi mdi-delete-outline"></i>
                    </button>
                </li>`;
        });
    }
    new bootstrap.Modal(document.getElementById('allUsersModal')).show();
}

async function deletePermissionAccess(endpointId, permissionId) {
    const result = await Swal.fire({
        title: 'Emin misiniz?',
        text: "Bu yetki grubunun bu endpoint'e erişim izni kaldırılacak!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Evet, kaldır!',
        cancelButtonText: 'Vazgeç'
    });

    if (result.isConfirmed) {
        try {
            await api.delete(`/Management/remove-permission-from-endpoint/${endpointId}/${permissionId}`);
            Swal.fire({
                icon: 'success',
                title: 'Kaldırıldı!',
                text: 'Yetki eşleşmesi başarıyla silindi.',
                timer: 1500,
                showConfirmButton: false
            });
            setTimeout(() => location.reload(), 1500);
        } catch (err) {
            Swal.fire('Hata!', err.response?.data?.message || 'İşlem başarısız.', 'error');
        }
    }
}

function filterModalList() {
    const query = document.getElementById('modalListSearch').value.toLowerCase();
    const items = document.querySelectorAll('#allUsersList .list-group-item');

    items.forEach(item => {
        const text = item.textContent.toLowerCase();
        if (text.includes(query)) {
            item.style.setProperty('display', 'flex', 'important');
        } else {
            item.style.setProperty('display', 'none', 'important');
        }
    });
}


document.addEventListener('DOMContentLoaded', loadEndpoints);