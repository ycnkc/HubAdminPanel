document.addEventListener('DOMContentLoaded', function () {
    console.log("Token Yönetimi sayfası hazır.");
    loadActiveTokens(); // Sayfa açıldığında veritabanındaki tokenları getir
});

// 1. Veritabanındaki Aktif Tokenları Listeleme
async function loadActiveTokens() {
    const tableBody = document.getElementById('tokenTableBody');
    if (!tableBody) return;

    try {
        // Backend'deki yeni yazacağımız GetTokens ucuna istek atıyoruz
        const response = await api.get('/Auth/tokens');

        if (response.data && response.data.length > 0) {
            tableBody.innerHTML = response.data.map(token => `
                <tr>
                    <td>
                        <div class="d-flex align-items-center">
                            <div class="avatar avatar-xs me-2">
                                <span class="avatar-initial rounded-circle bg-label-primary">
                                    <i class="mdi mdi-api"></i>
                                </span>
                            </div>
                            <div>
                                <span class="fw-medium">${token.name}</span>
                                <div class="text-muted small">****${token.tokenLastFour}</div>
                            </div>
                        </div>
                    </td>
                    <td>${new Date(token.createdDate).toLocaleDateString('tr-TR')}</td>
                    <td>${new Date(token.expireDate).toLocaleDateString('tr-TR')}</td>
                    <td>
                        <span class="badge ${token.isActive ? 'bg-label-success' : 'bg-label-secondary'} me-1">
                            ${token.isActive ? 'Aktif' : 'Pasif'}
                        </span>
                    </td>
                    <td>
                        <button type="button" class="btn btn-sm btn-icon btn-outline-danger" onclick="revokeToken(${token.id})">
                            <i class="mdi mdi-delete-outline"></i>
                        </button>
                    </td>
                </tr>
            `).join('');
        } else {
            tableBody.innerHTML = '<tr><td colspan="5" class="text-center p-4 text-muted">Henüz oluşturulmuş bir token bulunamadı.</td></tr>';
        }
    } catch (error) {
        console.error("Tokenlar yüklenirken hata:", error);
        tableBody.innerHTML = '<tr><td colspan="5" class="text-center text-danger p-4">Veriler yüklenemedi! Yetkinizi kontrol edin.</td></tr>';
    }
}

// 2. Yeni Token Üretme (Hashing ve Yetki Kontrollü)
async function generateNewToken() {
    const nameInput = document.getElementById('tokenName');
    const daysInput = document.getElementById('expireDate');

    if (!nameInput.value) {
        Swal.fire('Uyarı', 'Lütfen bir servis adı girin!', 'warning');
        return;
    }

    const requestData = {
        name: nameInput.value,
        expireDays: parseInt(daysInput.value)
    };

    try {
        const response = await api.post('/Auth/generate', requestData);

        if (response.data.success) {
            // 🎨 Başarı mesajı ve Orijinal Token'ı (Plain Text) sadece burada gösteriyoruz
            Swal.fire({
                title: 'Token Başarıyla Üretildi!',
                icon: 'success',
                html: `
                    <div class="text-start mt-3">
                        <p class="mb-2"><strong>Dikkat:</strong> Bu anahtarı şimdi kopyalayın. Güvenliğiniz için bir daha gösterilmeyecektir!</p>
                        <code class="p-3 bg-light d-block border rounded mb-3 text-primary" style="font-size: 1.1rem; word-break: break-all;">
                            ${response.data.token}
                        </code>
                        <p class="text-muted small">Veritabanında hash'lenerek saklanmıştır.</p>
                    </div>
                `,
                confirmButtonText: 'Kopyaladım ve Kaydettim'
            });

            // Modalı kapat ve formu temizle
            const modal = bootstrap.Modal.getInstance(document.getElementById('createTokenModal'));
            if (modal) modal.hide();
            nameInput.value = '';

            // Tabloyu yeniden yükle (Yeni veri listeye gelsin)
            loadActiveTokens();
        }
    } catch (error) {
        if (error.response && error.response.status === 403) {
            Swal.fire('Yetki Reddedildi', 'Token oluşturma yetkiniz (Token.Access) bulunmuyor!', 'error');
        } else {
            Swal.fire('Hata', 'İşlem sırasında bir hata oluştu.', 'error');
        }
    }
}

// 3. Token İptal Etme (Opsiyonel ama Mentorun bayılır)
async function revokeToken(id) {
    const result = await Swal.fire({
        title: 'Emin misiniz?',
        text: "Bu token iptal edilecek ve bir daha kullanılamayacak!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Evet, İptal Et',
        cancelButtonText: 'Vazgeç'
    });

    if (result.isConfirmed) {
        try {
            await api.delete(`/Auth/tokens/${id}`);
            Swal.fire('İptal Edildi!', 'Token başarıyla pasife alındı.', 'success');
            loadActiveTokens();
        } catch (error) {
            Swal.fire('Hata', 'Token iptal edilemedi.', 'error');
        }
    }
}