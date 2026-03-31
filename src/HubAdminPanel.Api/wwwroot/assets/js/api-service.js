/**
 * Axios instance pre-configured with the base URL and default headers.
 * This serves as the primary gateway for communicating with the HubAdminPanel API.
 */
const api = axios.create({
    baseURL: 'https://localhost:7016/api',
    headers: {
        'Content-Type': 'application/json'
    }
});

/**
 * Request Interceptor:
 * Automatically injects the JWT Access Token into the Authorization header for every outgoing request.
 * This ensures that protected endpoints (like Users or Roles) recognize the authenticated user.
 */
api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('accessToken');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

/**
 * Response Interceptor:
 * Globally handles API responses and manages session expiration errors.
 */
api.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response) {
            if (error.response.status === 401) {
                localStorage.clear();
                alert("Oturumunuz sona erdi, lütfen tekrar giriş yapın.");
                window.location.href = 'login.html';
            }

            else if (error.response.status === 403) {
                Swal.fire({
                    icon: 'error',
                    title: 'Yetkisiz İşlem!',
                    text: 'Bu işlemi gerçekleştirmek için gerekli yetkiye sahip değilsiniz.',
                    confirmButtonColor: '#d33',
                    confirmButtonText: 'Tamam'
                });
            }
        }

        return Promise.reject(error);
    }
);