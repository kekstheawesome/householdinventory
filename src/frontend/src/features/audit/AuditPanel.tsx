import { useEffect, useState } from 'react';
import api from '../../api/client';
import { Card } from '../../components/Card';
import { useAuth } from '../../contexts/AuthContext';
import type { AuditEntry } from '../../types';

export function AuditPanel() {
  const { auth } = useAuth();
  const [entries, setEntries] = useState<AuditEntry[]>([]);
  const [error, setError] = useState<string | null>(null);
  useEffect(() => {
    if (auth?.role !== 'Admin') return;
    const controller = new AbortController();
    api.get<AuditEntry[]>('/audit', { signal: controller.signal })
      .then((res) => setEntries(res.data))
      .catch((err) => {
        if (!controller.signal.aborted) setError('Failed to load audit log.');
      });
    return () => controller.abort();
  }, [auth?.role]);
  if (auth?.role !== 'Admin') return null;
  if (error) return <Card title="Recent Audit Activity"><span className="muted">{error}</span></Card>;
  return <Card title="Recent Audit Activity">{entries.slice(0, 8).map((entry) => <div key={entry.id} className="list-row">{entry.summary} <span className="muted">{entry.userEmail}</span></div>)}</Card>;
}
