import { useState } from 'react';
import { searchNotes } from '../api';

export default function SearchBar({ onResults, onClear }) {
  const [query, setQuery] = useState('');
  const [searching, setSearching] = useState(false);

  const handleSearch = async () => {
    if (!query.trim()) { onClear(); return; }
    setSearching(true);
    try {
      const res = await searchNotes(query);
      onResults(res.data);
    } catch {
      onResults([]);
    } finally {
      setSearching(false);
    }
  };

  const handleClear = () => {
    setQuery('');
    onClear();
  };

  return (
    <div style={{ display: 'flex', gap: '8px', marginBottom: '20px' }}>
      <input
        value={query}
        onChange={e => setQuery(e.target.value)}
        onKeyDown={e => e.key === 'Enter' && handleSearch()}
        placeholder="Search notes by meaning..."
        style={{
          flex: 1, padding: '10px 12px', fontSize: '14px',
          border: '1px solid #e2e8f0', borderRadius: '6px'
        }}
      />
      <button
        onClick={handleSearch}
        disabled={searching}
        style={{
          background: '#48bb78', color: '#fff', border: 'none',
          padding: '10px 16px', borderRadius: '6px', cursor: 'pointer', fontWeight: 600
        }}
      >
        {searching ? '...' : '🔍 Search'}
      </button>
      {query && (
        <button
          onClick={handleClear}
          style={{
            background: '#edf2f7', color: '#4a5568', border: 'none',
            padding: '10px 16px', borderRadius: '6px', cursor: 'pointer'
          }}
        >
          Clear
        </button>
      )}
    </div>
  );
}
