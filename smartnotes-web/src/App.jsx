import { useState, useEffect } from 'react';
import { getNotes, createNote, updateNote, deleteNote } from './api';
import NoteCard from './components/NoteCard';
import NoteEditor from './components/NoteEditor';
import SearchBar from './components/SearchBar';

export default function App() {
  const [notes, setNotes] = useState([]);
  const [filtered, setFiltered] = useState(null);
  const [selected, setSelected] = useState(null);
  const [showEditor, setShowEditor] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const fetchNotes = async () => {
    setLoading(true);
    try {
      const res = await getNotes();
      setNotes(res.data);
    } catch {
      setError('Failed to load notes. Is the API running?');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchNotes(); }, []);

  const handleSave = async (data) => {
    try {
      if (selected?.id) {
        await updateNote(selected.id, data);
      } else {
        await createNote(data);
      }
      setShowEditor(false);
      setSelected(null);
      await fetchNotes();
    } catch {
      setError('Failed to save note.');
    }
  };

  const handleDelete = async (id) => {
    if (!confirm('Delete this note?')) return;
    try {
      await deleteNote(id);
      await fetchNotes();
    } catch {
      setError('Failed to delete note.');
    }
  };

  const handleSelect = (note) => {
    setSelected(note);
    setShowEditor(true);
  };

  const handleNew = () => {
    setSelected(null);
    setShowEditor(true);
  };

  const displayNotes = filtered ?? notes;

  return (
    <div style={{ minHeight: '100vh', backgroundColor: '#f7fafc', fontFamily: 'system-ui, sans-serif' }}>
      {/* Header */}
      <div style={{ background: '#2b6cb0', color: '#fff', padding: '16px 24px', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <div>
          <h1 style={{ margin: 0, fontSize: '24px', fontWeight: 700 }}>📝 SmartNotes</h1>
          <p style={{ margin: '2px 0 0', fontSize: '13px', opacity: 0.8 }}>Semantic note-taking powered by pgvector</p>
        </div>
        <button
          onClick={handleNew}
          style={{
            background: '#fff', color: '#2b6cb0', border: 'none',
            padding: '10px 18px', borderRadius: '6px', cursor: 'pointer',
            fontWeight: 700, fontSize: '14px'
          }}
        >
          + New Note
        </button>
      </div>

      <div style={{ maxWidth: '900px', margin: '0 auto', padding: '24px 16px' }}>
        {error && (
          <div style={{ background: '#fff5f5', border: '1px solid #feb2b2', color: '#c53030', padding: '12px', borderRadius: '6px', marginBottom: '16px' }}>
            {error}
            <button onClick={() => setError('')} style={{ float: 'right', background: 'none', border: 'none', cursor: 'pointer', color: '#c53030' }}>×</button>
          </div>
        )}

        {showEditor ? (
          <NoteEditor
            note={selected}
            onSave={handleSave}
            onCancel={() => { setShowEditor(false); setSelected(null); }}
          />
        ) : (
          <>
            <SearchBar
              onResults={setFiltered}
              onClear={() => setFiltered(null)}
            />

            {filtered !== null && (
              <p style={{ color: '#718096', fontSize: '14px', marginBottom: '12px' }}>
                {filtered.length} search result{filtered.length !== 1 ? 's' : ''}
              </p>
            )}

            {loading ? (
              <p style={{ textAlign: 'center', color: '#a0aec0', padding: '40px' }}>Loading notes...</p>
            ) : displayNotes.length === 0 ? (
              <div style={{ textAlign: 'center', padding: '60px', color: '#a0aec0' }}>
                <p style={{ fontSize: '48px', margin: '0 0 12px' }}>📄</p>
                <p style={{ fontSize: '16px' }}>{filtered !== null ? 'No matching notes found' : 'No notes yet — create your first one!'}</p>
              </div>
            ) : (
              displayNotes.map(note => (
                <NoteCard
                  key={note.id}
                  note={note}
                  onSelect={handleSelect}
                  onDelete={handleDelete}
                />
              ))
            )}
          </>
        )}
      </div>
    </div>
  );
}
