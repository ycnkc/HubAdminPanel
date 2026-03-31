//fetching roles
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

        console.log("Roller dinamik olarak yüklendi!");
    } catch (error) {
        console.error("Roller yüklenirken bir hata oluştu:", error);
    }
}


//fetching permissions
async function fetchPermissions() {
    const container = document.getElementById('permissionsChecklist');
    try {
        const response = await api.get('Permissions');
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


async function createNewRole() {
    const roleName = document.getElementById('newRoleName').value;

    const selectedPermissions = Array.from(document.querySelectorAll('.perm-checkbox:checked'))
        .map(cb => parseInt(cb.value));

    if (!roleName) {
        Swal.fire('Hata', 'Lütfen rol adı giriniz.', 'warning');
        return;
    }

    const command = {
        name: roleName,
        permissionIds: selectedPermissions
    };

    try {
        await api.post('/Roles', command);

        Swal.fire('Başarılı', 'Yeni rol ve yetkileri oluşturuldu!', 'success');

        bootstrap.Modal.getInstance(document.getElementById('addRoleModal')).hide();
        await fetchRoles();

    } catch (error) {
        console.error("Rol oluşturma hatası:", error);
        Swal.fire('Hata', 'Rol oluşturulamadı.', 'error');
    }
}



async function updateRole(roleId) {
    const roleName = document.getElementById('editRoleName').value;
    const selectedPermissions = Array.from(document.querySelectorAll('.edit-perm-checkbox:checked'))
        .map(cb => parseInt(cb.value));

    const command = {
        id: roleId,
        name: roleName,
        permissionIds: selectedPermissions
    };

    try {
        await api.put(`/Roles/${roleId}`, command);
        Swal.fire('Başarılı', 'Rol güncellendi!', 'success');
        // Tabloyu ve modalı güncelle...
    } catch (error) {
        console.error("Güncelleme hatası:", error);
    }
}