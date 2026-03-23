import { useEffect, useState } from 'react';
import api from '../../api/client';
import { Card } from '../../components/Card';
import { useAuth } from '../../contexts/AuthContext';

export function UserPanel() {
  const { auth } = useAuth();
  const [users, setUsers] = useState<any[]>([]);
  useEffect(() => { if (auth?.role === 'Admin') api.get('/users').then((res) => setUsers(res.data)); }, [auth?.role]);
  if (auth?.role !== 'Admin') return null;
  return <Card title="Users">{users.map((user) => <div key={user.id} className="list-row">{user.fullName} <span className="muted">{user.email} · {user.roles.join(', ')}</span></div>)}</Card>;
}
