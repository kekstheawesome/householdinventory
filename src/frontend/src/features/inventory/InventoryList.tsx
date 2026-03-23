import { useEffect, useState } from 'react';
import api from '../../api/client';
import type { InventoryItem } from '../../types';
import { Card } from '../../components/Card';

export function InventoryList() {
  const [items, setItems] = useState<InventoryItem[]>([]);
  useEffect(() => { api.get('/inventory').then((res) => setItems(res.data)); }, []);

  async function adjust(id: string, route: 'restock' | 'consume') {
    await api.post(`/inventory/${id}/${route}`, { amount: 1, reason: route });
    const { data } = await api.get('/inventory');
    setItems(data);
  }

  return (
    <Card title="Inventory">
      <table className="table">
        <thead><tr><th>Item</th><th>Category</th><th>Qty</th><th>Status</th><th>Actions</th></tr></thead>
        <tbody>
          {items.map((item) => (
            <tr key={item.id}>
              <td>{item.name}<div className="muted">{item.location}</div></td>
              <td>{item.category}</td>
              <td>{item.quantity} {item.unit}</td>
              <td><span className={item.isOutOfStock ? 'badge danger' : item.isLowStock ? 'badge warn' : 'badge ok'}>{item.isOutOfStock ? 'Out' : item.isLowStock ? 'Low' : 'Healthy'}</span></td>
              <td><button onClick={() => adjust(item.id, 'consume')}>Used</button><button onClick={() => adjust(item.id, 'restock')}>Restocked</button></td>
            </tr>
          ))}
        </tbody>
      </table>
    </Card>
  );
}
