export function Card({ title, children }: { title: string; children: React.ReactNode }) {
  return <section className="card"><div className="card-title">{title}</div>{children}</section>;
}
