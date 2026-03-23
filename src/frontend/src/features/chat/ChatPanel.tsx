import { useState } from 'react';
import api from '../../api/client';
import { Card } from '../../components/Card';

export function ChatPanel() {
  const [question, setQuestion] = useState('What items are low?');
  const [answer, setAnswer] = useState('');

  return (
    <Card title="AI Household Assistant">
      <div className="stack">
        <textarea value={question} onChange={(e) => setQuestion(e.target.value)} rows={4} />
        <button onClick={async () => {
          const { data } = await api.post('/chat', { question });
          setAnswer(data.answer);
        }}>Ask Gemini</button>
        <div className="answer">{answer || 'Ask about low stock, audit history, or what to buy this week.'}</div>
      </div>
    </Card>
  );
}
