function getInitials(name) {
    if (!name) return "U";
    const names = name.trim().split(' ');
    let initials = names[0].substring(0, 1).toUpperCase();
    if (names.length > 1) {
        initials += names[names.length - 1].substring(0, 1).toUpperCase();
    }
    return initials;
}

function updateNavbarUserInfo() {
    const userName = localStorage.getItem('username') || 'User';
    const userRole = localStorage.getItem('userRole') || 'Guest';
    const initials = getInitials(userName);

    document.querySelectorAll('#navUserName').forEach(el => el.innerText = userName);
    document.querySelectorAll('#navUserRole').forEach(el => el.innerText = userRole);

    const initialsElements = document.querySelectorAll('.avatar-initial');
    initialsElements.forEach(el => {
        el.innerText = initials;
    });
}

document.addEventListener('DOMContentLoaded', () => {
    updateNavbarUserInfo();

    const logoutBtn = document.querySelector('.btn-logout');
    if (logoutBtn) {
        logoutBtn.addEventListener('click', logout);
    }
});