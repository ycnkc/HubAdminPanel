// Session, Token and Role control
const checkAuth = () => {
    const token = localStorage.getItem('accessToken');
    const userRole = localStorage.getItem('userRole'); 

    if (!token) {
        console.warn("Oturum bulunamadı: Login sayfasına yönlendiriliyor...");
        window.location.href = 'login.html';
        return null;
    }

    if (userRole !== 'Admin') {
        console.error("Yetkisiz erişim denemesi: Sadece Admin girişi yapabilir.");

        localStorage.clear();
        alert("Bu panele erişim yetkiniz bulunmamaktadır.");
        window.location.href = 'login.html';
        return null;
    }

    return token;
};

(function () {
    checkAuth();
})();

function handleLogout() {
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