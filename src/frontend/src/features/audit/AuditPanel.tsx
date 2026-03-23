import { useEffect, useState } from 'react';
import api from '../../api/client';
import { Card } from '../../components/Card';
import { useAuth } from '../../contexts/AuthContext';

export function AuditPanel() {
  const { auth } = useAuth();
  const [entries, setEntries] = useState<any[]>([]);
  useEffect(() => {
    if (auth?.role === 'Admin') api.get('/audit').then((res) => setEntries(res.data));
  }, [auth?.role]);
  if (auth?.role !== 'Admin') return null;
  return <Card title="Recent Audit Activity">{entries.slice(0, 8).map((entry) => <div key={entry.id} className="list-row">{entry.summary} <span className="muted">{entry.userEmail}</span></div>)}</Card>;
}
