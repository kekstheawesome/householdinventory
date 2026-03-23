import { AuthProvider, useAuth } from './contexts/AuthContext';
import { LoginForm } from './features/auth/LoginForm';
import { Dashboard } from './features/dashboard/Dashboard';
import { InventoryList } from './features/inventory/InventoryList';
import { ShoppingList } from './features/shopping/ShoppingList';
import { AuditPanel } from './features/audit/AuditPanel';
import { UserPanel } from './features/users/UserPanel';
import { ChatPanel } from './features/chat/ChatPanel';

function Shell() {
  const { auth, logout } = useAuth();
  if (!auth) return <LoginForm />;
  return (
    <div className="app-shell">
      <header className="topbar">
        <div>
          <h1>Household Inventory Dashboard</h1>
          <p>{auth.fullName} · {auth.role}</p>
        </div>
        <button onClick={logout}>Logout</button>
      </header>
      <Dashboard />
      <div className="layout-two">
        <InventoryList />
        <div className="stack"><ShoppingList /><ChatPanel /></div>
      </div>
      <div className="layout-two"><AuditPanel /><UserPanel /></div>
    </div>
  );
}

export function App() {
  return <AuthProvider><Shell /></AuthProvider>;
}
