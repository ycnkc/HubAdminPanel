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

function checkUIByPermissions() {
    const userPermissions = JSON.parse(localStorage.getItem('userPermissions') || '[]');

    const userRole = localStorage.getItem('userRole');
    if (userRole === 'Admin') return;

    document.querySelectorAll('.btn-perm').forEach(el => {
        const required = el.getAttribute('data-permission');
        if (!userPermissions.includes(required)) {
            el.style.display = 'none';
        }
    });
}

function logout() {
    Swal.fire({
        title: 'Çıkış Yapılıyor',
        text: "Oturumunuzu sonlandırmak istediğinize emin misiniz?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#696cff',
        cancelButtonColor: '#8592a3',
        confirmButtonText: 'Evet, çıkış yap',
        cancelButtonText: 'Vazgeç'
    }).then((result) => {
        if (result.isConfirmed) {
            localStorage.clear();

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