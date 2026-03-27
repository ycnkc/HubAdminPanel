const api = axios.create({
    baseURL: 'https://localhost:7016/api',
    headers: {
        'Content-Type': 'application/json'
    }
});

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

api.interceptors.response.use(
    (response) => response, 
    (error) => {
        if (error.response && error.response.status === 401) {
            
            alert("Oturumunuz sona erdi, lütfen tekrar giriş yapın.");
            localStorage.clear();
            window.location.href = 'login.html';
        }
        return Promise.reject(error);
    }
);