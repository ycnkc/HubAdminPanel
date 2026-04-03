/**
 * HubAdminPanel - Roles Management (Materio Version)
 */
let allPermissions = [];
const roleModalElement = document.getElementById('roleModal');
let roleModal;

document.addEventListener('DOMContentLoaded', () => {
    if (roleModalElement) {
        roleModal = new bootstrap.Modal(roleModalElement);
    }

    fetchRoles();
    loadPermissions();
});

async function prepareEdit(id, name) {
    const idField = document.getElementById('roleIdInput');
    const nameField = document.getElementById('roleNameInput');

    if (idField) idField.value = id;
    if (nameField) nameField.value = name;

    try {
        const response = await api.get(`/Roles/${id}`);
        const roleData = response.data || response;

        const currentPerms = roleData.rolePermissions || roleData.permissions || [];
        const currentIds = currentPerms.map(p => p.permissionId || p.id || p.Id);

        console.log("Seçili gelmesi gereken ID'ler:", currentIds);

        await loadPermissions(currentIds);

        const modal = bootstrap.Modal.getOrCreateInstance(document.getElementById('roleModal'));
        modal.show();

    } catch (error) {
        console.error("Hata:", error);
    }
}

async function fetchRoles() {
    try {
        console.log("Roller getiriliyor...");
        const response = await api.get('/Roles');

        const roles = Array.isArray(response) ? response : (response.data || []);

        const tbody = document.getElementById('roleTableBody');
        const totalRolesElem = document.getElementById('totalRoles');

        if (!tbody) {
            console.error("HATA: 'roleTableBody' ID'li element HTML içinde bulunamadı!");
            return;
        }

        if (totalRolesElem) totalRolesElem.innerText = roles.length;

        if (roles.length === 0) {
            tbody.innerHTML = '<tr><td colspan="4" class="text-center p-4">Henüz hiç rol tanımlanmamış.</td></tr>';
            return;
        }

        tbody.innerHTML = roles.map(role => {
            const permissions = role.permissions || role.Permissions || [];
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
                    ${permissions.length} Yetki
                </span>
            </td>
            <td>
                <div class="d-flex align-items-center">
                    <span class="badge badge-dot bg-success me-1"></span>
                    <small>Aktif</small>
                </div>
            </td>
            <td>
                <div class="dropdown">
                    <button type="button" class="btn p-0 dropdown-toggle hide-arrow" data-bs-toggle="dropdown" aria-expanded="false">
                        <i class="mdi mdi-dots-vertical mdi-24px"></i>
                    </button>
                    <div class="dropdown-menu dropdown-menu-end">
                        <a class="dropdown-item" href="javascript:void(0);"
                            onclick="prepareEdit(${roleId}, '${roleName}')">
                            <i class="mdi mdi-pencil-outline me-1"></i> Düzenle
                          </a>
                        <a class="dropdown-item text-danger" href="javascript:void(0);" 
                           onclick="deleteRole(${roleId})">
                            <i class="mdi mdi-trash-can-outline me-1"></i> Sil
                        </a>
                    </div>

                </div>
            </td>
        </tr>
    `;
        }).join('');

    } catch (error) {
        console.error("Rol listesi yüklenirken hata oluştu:", error);
    }
}

/**
 * @param {Array} selectedIds
 */
async function loadPermissions(selectedIds = []) {
    const container = document.getElementById('permissionList');
    if (!container) {
        console.error("Hata: 'permissionList' ID'li element bulunamadı!");
        return;
    }

    try {
        container.innerHTML = '<div class="text-center p-3"><div class="spinner-border spinner-border-sm text-primary" role="status"></div><span class="ms-2">Yetkiler yükleniyor...</span></div>';

        const response = await api.get('/Permissions');

        let permissions = [];
        if (response && response.data) {
            permissions = response.data; 
        } else if (Array.isArray(response)) {
            permissions = response;
        }

        if (!permissions || permissions.length === 0) {
            container.innerHTML = '<div class="alert alert-warning p-2 small">Sistemde tanımlı yetki bulunamadı.</div>';
            return;
        }

        container.innerHTML = permissions.map(p => {
            const pId = p.id;
            const pName = p.key;

            const isChecked = selectedIds.some(selectedId => {
                const sId = typeof selectedId === 'object' ? (selectedId.id || selectedId.Id) : selectedId;
                return String(sId) === String(pId); 
            }) ? 'checked' : '';

            return `
                <div class="d-flex justify-content-between align-items-center border-bottom py-2 px-1">
                    <label class="form-check-label fw-medium mb-0" for="perm${pId}" style="cursor:pointer;">
                        ${pName}
                    </label>
                    <div class="form-check mb-0">
                        <input class="form-check-input perm-cb" type="checkbox" 
                               value="${pId}" id="perm${pId}" ${isChecked} />
                    </div>

                </div>`;
        }).join('');

    } catch (error) {
        console.error("Yetki yükleme hatası:", error);
        container.innerHTML = '<div class="alert alert-danger p-2 small">Yetki servisiyle bağlantı kurulamadı!</div>';
    }
}

function openAddRoleModal() {
    document.getElementById('roleIdInput').value = '';
    document.getElementById('roleNameInput').value = '';
    document.getElementById('modalTitle').innerText = 'Yeni Rol Oluştur';

    loadPermissions([]);
    roleModal.show();
}


async function deleteRole(id) {
    const result = await Swal.fire({
        title: 'Emin misiniz?',
        text: "Bu rolü sildiğinizde geri alamazsınız!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Evet, sil!',
        cancelButtonText: 'İptal',
        customClass: {
            confirmButton: 'btn btn-danger me-3',
            cancelButton: 'btn btn-label-secondary'
        },
        buttonsStyling: false
    });

    if (result.isConfirmed) {
        try {
            await api.delete(`/Roles/${id}`);
            Swal.fire('Silindi!', 'Rol başarıyla silindi.', 'success');
            fetchRoles(); 
        } catch (error) {
            Swal.fire('Hata', 'Silme işlemi başarısız oldu.', 'error');
        }
    }
}

async function saveRolePermissions() {
    const roleId = document.getElementById('roleIdInput').value;
    const roleName = document.getElementById('roleNameInput').value;
    const selectedCheckboxes = document.querySelectorAll('.perm-cb:checked');
    const permissionIds = Array.from(selectedCheckboxes).map(cb => parseInt(cb.value));

    if (!roleName) {
        Swal.fire('Uyarı', 'Rol adı boş olamaz!', 'warning');
        return;
    }

    const updateDto = {
        id: parseInt(roleId),
        name: roleName,
        permissionIds: permissionIds
    };

    try {
        const response = await api.post(`/Roles/${roleId}`, updateDto);

        Swal.fire({
            title: 'Başarılı!',
            text: 'Rol yetkileri başarıyla güncellendi.',
            icon: 'success',
            confirmButtonText: 'Tamam',
            timer: 2000
        });

        const modalElement = document.getElementById('roleModal');
        const modal = bootstrap.Modal.getInstance(modalElement);
        if (modal) modal.hide();

        if (typeof fetchRoles === 'function') {
            fetchRoles();
        }
    } catch (error) {
        console.error("Güncelleme hatası:", error);
        Swal.fire('Hata', 'Yetkiler kaydedilirken bir sorun oluştu!', 'error');
    }
}