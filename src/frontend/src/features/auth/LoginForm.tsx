import { useState } from 'react';
import { useAuth } from '../../contexts/AuthContext';

export function LoginForm() {
  const { login } = useAuth();
  const [email, setEmail] = useState('admin@household.local');
  const [password, setPassword] = useState('Admin123!');

  return (
    <div className="login-shell">
      <form className="card login-card" onSubmit={async (e) => { e.preventDefault(); await login(email, password); }}>
        <h1>Household Inventory</h1>
        <p>Shared home stock, alerts, shopping, and AI assistance.</p>
        <input value={email} onChange={(e) => setEmail(e.target.value)} placeholder="Email" />
        <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} placeholder="Password" />
        <button type="submit">Sign in</button>
      </form>
    </div>
  );
}
