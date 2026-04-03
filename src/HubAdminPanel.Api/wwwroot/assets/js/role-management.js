async function fetchRoles() {
    try {
        const response = await api.get('Roles');
        const roles = response.data;

        const selects = {
            add: document.getElementById('newUserRole'),
            edit: document.getElementById('editUserRole'),
            filter: document.getElementById('filterRole')
        };

        Object.keys(selects).forEach(key => {
            const select = selects[key];
            if (!select) return;

            select.innerHTML = '';

            if (key === 'filter') {
                const defaultOpt = document.createElement('option');
                defaultOpt.value = "";
                defaultOpt.textContent = "Tüm Roller";
                select.appendChild(defaultOpt);
            }

            roles.forEach(role => {
                const option = document.createElement('option');
                option.value = role.id || role.Id;
                option.textContent = role.name || role.Name;
                select.appendChild(option);
            });
        });
    } catch (error) {
        console.error("Roller yüklenirken bir hata oluştu:", error);
    }
}

async function fetchEndpoints() {
    const container = document.getElementById('endpointList');
    if (!container) return;

    try {
        const response = await api.get('Management/endpoints');
        const endpoints = response.data;

        container.innerHTML = endpoints.map(ep => `
            <div class="form-check mb-2">
                <input class="form-check-input endpoint-checkbox" type="checkbox" value="${ep.id}" id="ep_${ep.id}">
                <label class="form-check-label d-flex justify-content-between w-100" for="ep_${ep.id}">
                    <span>
                        <span class="badge bg-label-primary me-2">${ep.method}</span>
                        <span class="text-dark fw-medium">${ep.path}</span>
                    </span>
                    <small class="text-muted italic">${ep.description || ''}</small>
                </label>
            </div>
        `).join('');
    } catch (e) {
        container.innerHTML = '<span class="text-danger">Endpointler yüklenemedi.</span>';
    }
}

async function createNewRole() {
    const roleName = document.getElementById('newRoleName').value.trim();
    const selectedEndpoints = Array.from(document.querySelectorAll('#addRoleModal .endpoint-checkbox:checked'))
        .map(cb => parseInt(cb.value));

    if (!roleName) {
        return Swal.fire('Uyarı', 'Lütfen bir rol adı giriniz.', 'warning');
    }

    const payload = {
        Name: roleName,
        EndpointIds: selectedEndpoints
    };

    try {
        await api.post('/Roles/create', payload);

        await Swal.fire({
            icon: 'success',
            title: 'Başarılı',
            text: 'Rol ve yetkiler başarıyla oluşturuldu.',
            timer: 1500,
            showConfirmButton: false
        });

        const modalElement = document.getElementById('addRoleModal');
        const modal = bootstrap.Modal.getInstance(modalElement);
        if (modal) modal.hide();

        location.reload();
    } catch (error) {
        console.error("Rol oluşturma hatası:", error);
        Swal.fire('Hata', 'Rol oluşturulurken bir sorun oluştu.', 'error');
    }
}

async function updateRole() {
    const roleId = document.getElementById('roleIdInput').value;
    const roleName = document.getElementById('roleNameInput').value.trim();
    const selectedEndpoints = Array.from(document.querySelectorAll('#roleModal .endpoint-checkbox:checked'))
        .map(cb => parseInt(cb.value));

    if (!roleName) {
        return Swal.fire('Uyarı', 'Rol adı boş olamaz.', 'warning');
    }

    const payload = {
        Id: parseInt(roleId),
        Name: roleName,
        EndpointIds: selectedEndpoints
    };

    try {
        await api.put(`/Roles/${roleId}`, payload);

        await Swal.fire({
            icon: 'success',
            title: 'Başarılı',
            text: 'Rol ve erişim yetkileri güncellendi.',
            timer: 1500,
            showConfirmButton: false
        });

        const modalElement = document.getElementById('roleModal');
        const modal = bootstrap.Modal.getInstance(modalElement);
        if (modal) modal.hide();

        location.reload();
    } catch (error) {
        console.error("Güncelleme hatası:", error);
        Swal.fire('Hata', 'Güncelleme sırasında bir hata oluştu.', 'error');
    }
}

async function deleteRole(id) {
    const confirmation = await Swal.fire({
        title: 'Emin misiniz?',
        text: "Bu rol silindiğinde bağlı tüm yetkiler de kaldırılacaktır!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Evet, sil',
        cancelButtonText: 'İptal'
    });

    if (confirmation.isConfirmed) {
        try {
            await api.delete(`/Roles/${id}`);

            await Swal.fire('Başarılı', 'Rol silindi.', 'success');
            location.reload();
        } catch (error) {
            console.error(error);
            Swal.fire('Hata', 'Silme işlemi sırasında bir hata oluştu.', 'error');
        }
    }
}