import { useEffect, useState } from 'react';
import api from '../../api/client';
import { Card } from '../../components/Card';

export function ShoppingList() {
  const [items, setItems] = useState<any[]>([]);
  useEffect(() => { api.get('/shoppinglist').then((res) => setItems(res.data)); }, []);
  return <Card title="Shopping List">{items.map((item) => <div key={item.id} className="list-row">{item.name} - {item.quantity} {item.unit}</div>)}</Card>;
}
