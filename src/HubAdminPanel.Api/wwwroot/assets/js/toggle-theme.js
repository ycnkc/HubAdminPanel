const toggleTheme = () => {
    const htmlElement = document.documentElement;
    const currentTheme = htmlElement.getAttribute('data-bs-theme');
    const newTheme = currentTheme === 'light' ? 'dark' : 'light';

    htmlElement.setAttribute('data-bs-theme', newTheme);

    const icon = document.getElementById('themeIcon');
    icon.className = newTheme === 'dark' ? 'ri-sun-line icon-md' : 'ri-moon-line icon-md';

    localStorage.setItem('theme', newTheme);
};

document.addEventListener('DOMContentLoaded', () => {
    const savedTheme = localStorage.getItem('theme') || 'light';
    document.documentElement.setAttribute('data-bs-theme', savedTheme);

    const icon = document.getElementById('themeIcon');
    if (icon) {
        icon.className = savedTheme === 'dark' ? 'ri-sun-line icon-md' : 'ri-moon-line icon-md';
    }
});
