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
                        <button class="btn btn-sm btn-icon btn-outline-primary" 
                                onclick="openAssignModal(${ep.id}, '${ep.path}')" 
                                title="Yetki Ekle">
                            <i class="mdi mdi-plus-circle"></i>
                        </button>
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