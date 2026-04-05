const roleModalElement = document.getElementById('roleModal');
let roleModal;

document.addEventListener('DOMContentLoaded', () => {
    if (roleModalElement) {
        roleModal = new bootstrap.Modal(roleModalElement);
    }
    fetchRoles();
});

async function fetchRoles() {
    try {
        const response = await api.get('/Roles');
        const roles = Array.isArray(response) ? response : (response.data || []);

        const tbody = document.getElementById('roleTableBody');
        const totalRolesElem = document.getElementById('totalRoles');

        if (!tbody) return;
        if (totalRolesElem) totalRolesElem.innerText = roles.length;

        if (roles.length === 0) {
            tbody.innerHTML = '<tr><td colspan="4" class="text-center p-4">Henüz hiç rol tanımlanmamış.</td></tr>';
            return;
        }

        tbody.innerHTML = roles.map(role => {
            const mappings = role.endpointRoleMappings || [];
            const roleId = role.id || role.Id;
            const roleName = role.name || role.Name;

            return `
                <tr>
                    <td>
                        <div class="d-flex align-items-center">
                            <div class="avatar avatar-sm me-3">
                                <span class="avatar-initial rounded-circle bg-label-primary">
                                    <i class="mdi mdi-shield-outline"></i>
                                </span>
                            </div>
                            <span class="fw-semibold">${roleName}</span>
                        </div>
                    </td>
                    <td>
                        <span class="badge bg-label-info">
                            ${mappings.length} Endpoint Erişimi
                        </span>
                    </td>
                    
                    <td>
                        <div class="dropdown">
                            <button type="button" class="btn p-0 dropdown-toggle hide-arrow" data-bs-toggle="dropdown">
                                <i class="mdi mdi-dots-vertical mdi-24px"></i>
                            </button>
                            <div class="dropdown-menu dropdown-menu-end">
                                <a class="dropdown-item" href="javascript:void(0);" onclick="prepareEdit(${roleId}, '${roleName}')">
                                    <i class="mdi mdi-pencil-outline me-1"></i> Düzenle
                                </a>
                                <a class="dropdown-item text-danger" href="javascript:void(0);" onclick="deleteRole(${roleId})">
                                    <i class="mdi mdi-trash-can-outline me-1"></i> Sil
                                </a>
                            </div>
                        </div>
                    </td>
                </tr>`;
        }).join('');

    } catch (error) {
        console.error("Rol listesi yüklenirken hata:", error);
    }
}

async function loadEndpoints(selectedIds = []) {
    const container = document.getElementById('endpointList');
    if (!container) return;

    try {
        container.innerHTML = '<div class="text-center p-3"><div class="spinner-border spinner-border-sm text-primary"></div></div>';

        const response = await api.get('/Management/endpoints');
        const endpoints = Array.isArray(response) ? response : (response.data || []);

        if (endpoints.length === 0) {
            container.innerHTML = '<div class="alert alert-warning p-2 small">Sistemde kayıtlı endpoint bulunamadı.</div>';
            return;
        }

        const methodColors = {
            'GET': 'bg-label-success',
            'POST': 'bg-label-primary',
            'PUT': 'bg-label-warning',
            'DELETE': 'bg-label-danger'
        };

        let html = `
            <div class="form-check mb-3 pb-2 border-bottom">
                <input class="form-check-input" type="checkbox" id="selectAllEndpoints">
                <label class="form-check-label fw-bold" for="selectAllEndpoints">Tümünü Seç / Kaldır</label>
            </div>
        `;

        html += endpoints.map(ep => {
            const epId = ep.id || ep.Id;
            const isChecked = selectedIds.includes(epId) ? 'checked' : '';
            const badgeClass = methodColors[ep.method] || 'bg-label-secondary';

            return `
                <div class="d-flex justify-content-between align-items-center border-bottom py-2 px-1 endpoint-item">
                    <label class="form-check-label fw-medium mb-0" for="ep${epId}" style="cursor:pointer; flex-grow: 1;">
                        <span class="badge ${badgeClass} me-2" style="width: 60px;">${ep.method}</span>
                        <span class="text-dark">${ep.path}</span>
                        <div class="small text-muted italic" style="font-size: 0.75rem;">${ep.description || ''}</div>
                    </label>
                    <div class="form-check mb-0">
                        <input class="form-check-input endpoint-cb" type="checkbox" value="${epId}" id="ep${epId}" ${isChecked} />
                    </div>
                </div>`;
        }).join('');

        container.innerHTML = html;

        setupEndpointListeners();
        updateSelectedCount();

    } catch (error) {
        container.innerHTML = '<div class="alert alert-danger p-2 small">Endpoint yüklenemedi.</div>';
    }
}

function setupEndpointListeners() {
    const searchInput = document.getElementById('endpointSearch');
    const selectAll = document.getElementById('selectAllEndpoints');
    const checkboxes = document.querySelectorAll('.endpoint-cb');

    if (searchInput) {
        searchInput.addEventListener('input', (e) => {
            const searchTerm = e.target.value.toLowerCase();
            const items = document.querySelectorAll('.endpoint-item');

            items.forEach(item => {
                const text = item.innerText.toLowerCase();
                if (text.includes(searchTerm)) {
                    item.classList.remove('d-none');
                    item.classList.add('d-flex');
                } else {
                    item.classList.remove('d-flex');
                    item.classList.add('d-none');
                }
            });
        });
    }

    checkboxes.forEach(cb => {
        cb.addEventListener('change', updateSelectedCount);
    });

    if (selectAll) {
        selectAll.addEventListener('change', (e) => {
            checkboxes.forEach(cb => {
                const item = cb.closest('.endpoint-item');
                if (item && !item.classList.contains('d-none')) {
                    cb.checked = e.target.checked;
                }
            });
            updateSelectedCount();
        });
    }
}

function updateSelectedCount() {
    const count = document.querySelectorAll('.endpoint-cb:checked').length;
    const badge = document.getElementById('selectedCount');
    if (badge) {
        badge.innerText = `${count} endpoint seçildi`;
        badge.classList.toggle('text-primary', count > 0);
        badge.classList.toggle('fw-bold', count > 0);
    }
}

async function prepareEdit(id, name) {
    document.getElementById('roleIdInput').value = id;
    document.getElementById('roleNameInput').value = name;
    document.getElementById('modalTitle').innerText = 'Rolü Düzenle';

    try {
        const response = await api.get(`/Roles/${id}`);
        const roleData = response.data || response;

        const currentMappings = roleData.endpointRoleMappings || [];
        const currentIds = currentMappings.map(m => m.endpointId || m.EndpointId);

        await loadEndpoints(currentIds);
        roleModal.show();
    } catch (error) {
        console.error("Veri çekme hatası:", error);
    }
}

function openAddRoleModal() {
    document.getElementById('roleIdInput').value = '';
    document.getElementById('roleNameInput').value = '';
    document.getElementById('modalTitle').innerText = 'Yeni Rol Oluştur';
    loadEndpoints([]);
    roleModal.show();
}

async function saveRoleEndpoints() {
    const roleId = document.getElementById('roleIdInput').value;
    const roleName = document.getElementById('roleNameInput').value.trim();
    const selectedCheckboxes = document.querySelectorAll('.endpoint-cb:checked');
    const endpointIds = Array.from(selectedCheckboxes).map(cb => parseInt(cb.value));

    if (!roleName) {
        Swal.fire('Uyarı', 'Rol adı boş olamaz!', 'warning');
        return;
    }

    const payload = {
        id: roleId ? parseInt(roleId) : 0,
        name: roleName,
        endpointIds: endpointIds
    };

    try {
        if (roleId) {
            await api.put(`/Roles/${roleId}`, payload);
        } else {
            await api.post('/Roles/create', payload);
        }

        Swal.fire({ title: 'Başarılı!', text: 'Kaydedildi.', icon: 'success', timer: 1500, showConfirmButton: false });
        roleModal.hide();
        fetchRoles();
    } catch (error) {
        Swal.fire('Hata', 'İşlem başarısız.', 'error');
    }
}

async function deleteRole(id) {
    const result = await Swal.fire({
        title: 'Emin misiniz?',
        text: "Bu rolü sildiğinizde yetki eşleşmeleri de kalkacaktır!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Evet, sil!',
        cancelButtonText: 'İptal',
        customClass: { confirmButton: 'btn btn-danger me-3', cancelButton: 'btn btn-label-secondary' },
        buttonsStyling: false
    });

    if (result.isConfirmed) {
        try {
            await api.delete(`/Roles/${id}`);
            Swal.fire({ title: 'Silindi!', text: 'Başarıyla silindi.', icon: 'success', timer: 1500, showConfirmButton: false });
            fetchRoles();
        } catch (error) {
            Swal.fire('Hata', 'Silme başarısız.', 'error');
        }
    }
}