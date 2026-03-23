import axios from 'axios';

const api = axios.create({ baseURL: import.meta.env.VITE_API_BASE_URL ?? '/api' });
api.interceptors.request.use((config) => {
  try {
    const raw = localStorage.getItem('household-auth');
    if (raw) {
      const auth = JSON.parse(raw);
      if (auth?.token) config.headers.Authorization = `Bearer ${auth.token}`;
    }
  } catch {
    // Ignore malformed auth storage.
  }
  return config;
});
export default api;
