import { useEffect, useState } from 'react';
import api from '../../api/client';
import { Card } from '../../components/Card';

export function Dashboard() {
  const [data, setData] = useState<any>(null);
  useEffect(() => { api.get('/dashboard').then((res) => setData(res.data)); }, []);
  if (!data) return <Card title="Dashboard">Loading...</Card>;
  const stats = [
    ['Total Items', data.totalItems],
    ['Low Stock', data.lowStockCount],
    ['Out of Stock', data.outOfStockCount],
    ['Expiring Soon', data.expiringSoonCount],
    ['Shopping List', data.shoppingListCount]
  ];
  return <div className="dashboard-grid">{stats.map(([label, value]) => <Card key={label} title={label as string}><strong>{value}</strong></Card>)}</div>;
}
