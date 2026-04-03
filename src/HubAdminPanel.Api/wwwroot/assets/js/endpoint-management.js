document.addEventListener('DOMContentLoaded', function () {
    initialize();
});

let allEndpoints = [];
let filteredEndpoints = [];
let searchTimeout;

async function initialize() {
    await loadEndpoints();
    await loadRoles();
}

async function loadEndpoints(preservePage = false) {
    try {
        const response = await api.get('/Management/endpoints');
        allEndpoints = response.data;

        const pathQuery = document.getElementById('filterPath')?.value.toLowerCase() || "";
        const methodQuery = document.getElementById('filterMethod')?.value || "";

        filteredEndpoints = allEndpoints.filter(ep =>
            ep.path.toLowerCase().includes(pathQuery) &&
            (methodQuery === "" || ep.method === methodQuery)
        );

        // Eğer preservePage true ise mevcut sayfada kal, yoksa 1. sayfaya dön
        const targetPage = preservePage ? (PaginationManager.currentPage || 1) : 1;
        loadPage(targetPage);
    } catch (error) {
        console.error(error);
    }
}

function loadPage(pageNumber = 1) {
    const pageSize = PaginationManager.pageSize || 5;
    const startIndex = (pageNumber - 1) * pageSize;
    const endIndex = startIndex + pageSize;

    const pagedItems = filteredEndpoints.slice(startIndex, endIndex);

    renderTable(pagedItems);

    const pagedData = {
        items: pagedItems,
        totalCount: filteredEndpoints.length,
        pageNumber: pageNumber,
        pageSize: pageSize
    };

    PaginationManager.update(pagedData, loadPage);
}
window.loadPage = loadPage;

function renderTable(endpoints) {
    const tableBody = document.getElementById('endpointTableBody');
    if (!tableBody) return;
    tableBody.innerHTML = '';

    if (!endpoints || endpoints.length === 0) {
        tableBody.innerHTML = `<tr><td colspan="5" class="text-center p-4">Sonuç bulunamadı.</td></tr>`;
        return;
    }

    endpoints.forEach(ep => {
        const methods = { 'GET': 'bg-label-success', 'POST': 'bg-label-primary', 'PUT': 'bg-label-warning', 'DELETE': 'bg-label-danger' };
        const methodBadge = `<span class="badge ${methods[ep.method] || 'bg-label-secondary'}">${ep.method}</span>`;

        let rolesHtml = '';
        const roleCount = ep.endpointRoleMappings?.length || 0;

        if (roleCount > 0) {
            const firstRole = ep.endpointRoleMappings[0].role?.name || '???';

            rolesHtml = roleCount === 1
                ? `<div><span class="badge bg-label-info">${firstRole}</span></div>`
                : `<div><span class="badge bg-label-info">${firstRole}</span>
                   <a href="javascript:void(0);" onclick="showAllRoles(${ep.id})" class="small text-info fw-bold ms-1 text-decoration-underline">
                       ve ${roleCount - 1} rol daha
                   </a></div>`;
        }

        const finalAccessHtml = rolesHtml || '<span class="text-muted small italic">Atama yok</span>';

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
                                <i class="mdi mdi-shield-check-outline me-1"></i> Role Ata
                            </a>
                        </div>
                    </div>
                </td>
            </tr>`;
    });
}
async function loadRoles() {
    try {
        const response = await api.get('/Roles');
        const select = document.getElementById('roleSelect');
        select.innerHTML = '<option value="">Rol Seçiniz...</option>';
        response.data.forEach(r => select.innerHTML += `<option value="${r.id}">${r.name}</option>`);
    } catch (error) {
        console.error(error);
    }
}

function openAssignModal(id, path) {
    document.getElementById('selectedEndpointId').value = id;
    document.getElementById('endpointPathDisplay').value = path;
    new bootstrap.Modal(document.getElementById('assignRoleModal')).show();
}

async function saveRoleAssignment() {
    const endpointId = document.getElementById('selectedEndpointId').value;
    const roleId = document.getElementById('roleSelect').value;
    if (!roleId) return showToast('Uyarı', 'Lütfen bir rol seçin', 'warning');

    try {
        await api.post(`/Management/assign-role?endpointId=${endpointId}&roleId=${roleId}`);
        bootstrap.Modal.getInstance(document.getElementById('assignRoleModal')).hide();
        showToast('Başarılı', 'Rol başarıyla atandı', 'success');
        loadEndpoints(true);
    } catch (error) {
        showToast('Hata', error.response?.data || 'İşlem başarısız', 'error');
    }
}

function showAllRoles(endpointId) {
    const ep = allEndpoints.find(e => e.id === endpointId);
    const listElement = document.getElementById('allRolesList');
    document.querySelector('#allRolesModal .modal-title').innerText = "Erişim İzni Olan Roller";

    const searchInput = document.getElementById('modalListSearch');
    if (searchInput) {
        searchInput.value = '';
        searchInput.style.display = (ep.endpointRoleMappings && ep.endpointRoleMappings.length > 0) ? 'block' : 'none';
    }

    listElement.innerHTML = '';

    if (!ep.endpointRoleMappings || ep.endpointRoleMappings.length === 0) {
        listElement.innerHTML = '<li class="list-group-item text-muted small text-center">Herhangi bir rol atanmamış.</li>';
    } else {
        ep.endpointRoleMappings.forEach(m => {
            const roleName = m.role?.name || '???';
            listElement.innerHTML += `<li class="list-group-item d-flex justify-content-between align-items-center">
                <span class="user-name-text"><i class="mdi mdi-shield-outline me-2 text-info"></i>${roleName}</span>
                <button class="btn btn-sm btn-outline-danger border-0" onclick="deleteRoleAccess(${endpointId}, ${m.roleId})">
                    <i class="mdi mdi-delete-outline"></i>
                </button>
            </li>`;
        });
    }
    new bootstrap.Modal(document.getElementById('allRolesModal')).show();
}

async function deleteRoleAccess(endpointId, roleId) {
    const result = await Swal.fire({
        title: 'Emin misiniz?',
        text: "Bu rolün bu endpoint'e erişim izni kaldırılacak!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Evet, kaldır!',
        cancelButtonText: 'Vazgeç'
    });

    if (result.isConfirmed) {
        try {
            await api.delete(`/Management/remove-role-from-endpoint/${endpointId}/${roleId}`);
            Swal.fire({ icon: 'success', title: 'Kaldırıldı!', text: 'Rol eşleşmesi başarıyla silindi.', timer: 1500, showConfirmButton: false });
            await loadEndpoints(true);
            showAllRoles(endpointId);
        } catch (err) {
            Swal.fire('Hata!', err.response?.data?.message || 'İşlem başarısız.', 'error');
        }
    }
}

function showToast(title, text, icon) {
    Swal.fire({ title, text, icon, toast: true, position: 'top-end', timer: 3000, showConfirmButton: false });
}

function filterModalList() {
    const query = document.getElementById('modalListSearch').value.toLowerCase();
    document.querySelectorAll('#allRolesList .list-group-item').forEach(item => {
        const text = item.querySelector('.user-name-text')?.textContent.toLowerCase() || item.textContent.toLowerCase();
        if (text.includes(query)) {
            item.classList.remove('d-none');
            item.classList.add('d-flex');
        } else {
            item.classList.remove('d-flex');
            item.classList.add('d-none');
        }
    });
}



window.applyFilters = function () {
    const pathQuery = document.getElementById('filterPath')?.value.toLowerCase() || "";
    const methodQuery = document.getElementById('filterMethod')?.value || "";

    filteredEndpoints = allEndpoints.filter(ep =>
        ep.path.toLowerCase().includes(pathQuery) &&
        (methodQuery === "" || ep.method === methodQuery)
    );

    loadPage(1);
};