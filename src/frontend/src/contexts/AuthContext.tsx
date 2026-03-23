import { createContext, useContext, useMemo, useState } from 'react';
import api from '../api/client';
import type { AuthState } from '../types';

interface AuthContextValue {
  auth: AuthState | null;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [auth, setAuth] = useState<AuthState | null>(() => {
    const raw = localStorage.getItem('household-auth');
    return raw ? JSON.parse(raw) : null;
  });

  const value = useMemo(() => ({
    auth,
    async login(email: string, password: string) {
      const { data } = await api.post('/auth/login', { email, password });
      localStorage.setItem('household-auth', JSON.stringify(data));
      setAuth(data);
    },
    logout() {
      localStorage.removeItem('household-auth');
      setAuth(null);
    }
  }), [auth]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error('useAuth must be used within AuthProvider');
  return context;
}
