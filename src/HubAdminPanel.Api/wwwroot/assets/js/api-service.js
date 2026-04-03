/**
 * Axios instance pre-configured with the base URL and default headers.
 */
const api = axios.create({
    baseURL: 'https://localhost:7016/api',
    headers: {
        'Content-Type': 'application/json'
    }
});

let isRefreshing = false;
let failedQueue = [];

const processQueue = (error, token = null) => {
    failedQueue.forEach(prom => {
        if (error) {
            prom.reject(error);
        } else {
            prom.resolve(token);
        }
    });
    failedQueue = [];
};

/**
 * Request Interceptor:
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
 */
api.interceptors.response.use(
    (response) => response,
    async (error) => {
        const originalRequest = error.config;

        if (error.response) {
            if (error.response.status === 401 && !originalRequest._retry) {

                if (isRefreshing) {
                    return new Promise(function (resolve, reject) {
                        failedQueue.push({ resolve, reject });
                    }).then(token => {
                        originalRequest.headers['Authorization'] = 'Bearer ' + token;
                        return api(originalRequest);
                    }).catch(err => {
                        return Promise.reject(err);
                    });
                }

                originalRequest._retry = true;
                isRefreshing = true;

                const refreshToken = localStorage.getItem('refreshToken');

                if (!refreshToken) {
                    localStorage.clear();
                    alert("Oturumunuz sona erdi, lütfen tekrar giriş yapın.");
                    window.location.href = 'login.html';
                    return Promise.reject(error);
                }

                return new Promise(function (resolve, reject) {
                    axios.post('https://localhost:7016/api/Auth/refresh-token', {
                        refreshToken: refreshToken
                    })
                        .then(({ data }) => {
                            localStorage.setItem('accessToken', data.accessToken);
                            localStorage.setItem('refreshToken', data.refreshToken);

                            originalRequest.headers['Authorization'] = 'Bearer ' + data.accessToken;
                            processQueue(null, data.accessToken);
                            resolve(api(originalRequest)); 
                        })
                        .catch((err) => {
                            processQueue(err, null);
                            localStorage.clear();
                            alert("Oturumunuz sona erdi, lütfen tekrar giriş yapın.");
                            window.location.href = 'login.html';
                            reject(err);
                        })
                        .finally(() => {
                            isRefreshing = false;
                        });
                });
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