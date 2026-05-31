export default function NoteCard({ note, onSelect, onDelete }) {
  return (
    <div style={{
      border: '1px solid #e2e8f0',
      borderRadius: '8px',
      padding: '16px',
      marginBottom: '12px',
      cursor: 'pointer',
      backgroundColor: '#fff',
      boxShadow: '0 1px 3px rgba(0,0,0,0.1)'
    }}>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'start' }}>
        <h3
          onClick={() => onSelect(note)}
          style={{ margin: 0, color: '#2d3748', fontSize: '16px', fontWeight: 600 }}
        >
          {note.title}
        </h3>
        <button
          onClick={(e) => { e.stopPropagation(); onDelete(note.id); }}
          style={{
            background: 'none', border: 'none', color: '#e53e3e',
            cursor: 'pointer', fontSize: '18px', padding: '0 4px'
          }}
        >×</button>
      </div>
      <p style={{ color: '#718096', fontSize: '14px', margin: '8px 0 0', lineHeight: 1.5 }}>
        {note.content?.slice(0, 120)}{note.content?.length > 120 ? '...' : ''}
      </p>
      {note.tags?.length > 0 && (
        <div style={{ marginTop: '8px', display: 'flex', gap: '6px', flexWrap: 'wrap' }}>
          {note.tags.map(tag => (
            <span key={tag.id || tag.name} style={{
              background: '#ebf8ff', color: '#2b6cb0',
              padding: '2px 8px', borderRadius: '12px', fontSize: '12px'
            }}>
              {tag.name}
            </span>
          ))}
        </div>
      )}
    </div>
  );
}
