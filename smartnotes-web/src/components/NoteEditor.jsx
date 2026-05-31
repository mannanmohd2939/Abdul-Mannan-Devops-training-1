import { useState, useEffect } from 'react';
import { uploadFile } from '../api';

export default function NoteEditor({ note, onSave, onCancel }) {
  const [title, setTitle] = useState('');
  const [content, setContent] = useState('');
  const [saving, setSaving] = useState(false);
  const [uploading, setUploading] = useState(false);
  const [uploadMsg, setUploadMsg] = useState('');

  useEffect(() => {
    if (note) {
      setTitle(note.title || '');
      setContent(note.content || '');
    } else {
      setTitle('');
      setContent('');
    }
  }, [note]);

  const handleSave = async () => {
    if (!title.trim()) return;
    setSaving(true);
    try {
      await onSave({ title, content });
    } finally {
      setSaving(false);
    }
  };

  const handleUpload = async (e) => {
    const file = e.target.files[0];
    if (!file || !note?.id) {
      setUploadMsg('Save the note first before uploading a file.');
      return;
    }
    setUploading(true);
    setUploadMsg('');
    try {
      await uploadFile(note.id, file);
      setUploadMsg(`✅ ${file.name} uploaded successfully`);
    } catch {
      setUploadMsg('❌ Upload failed');
    } finally {
      setUploading(false);
    }
  };

  return (
    <div style={{ backgroundColor: '#fff', borderRadius: '8px', padding: '24px', boxShadow: '0 1px 3px rgba(0,0,0,0.1)' }}>
      <h2 style={{ margin: '0 0 16px', color: '#2d3748' }}>
        {note?.id ? 'Edit Note' : 'New Note'}
      </h2>

      <input
        value={title}
        onChange={e => setTitle(e.target.value)}
        placeholder="Note title..."
        style={{
          width: '100%', padding: '10px 12px', fontSize: '16px',
          border: '1px solid #e2e8f0', borderRadius: '6px',
          marginBottom: '12px', boxSizing: 'border-box'
        }}
      />

      <textarea
        value={content}
        onChange={e => setContent(e.target.value)}
        placeholder="Write your note here..."
        rows={8}
        style={{
          width: '100%', padding: '10px 12px', fontSize: '14px',
          border: '1px solid #e2e8f0', borderRadius: '6px',
          marginBottom: '12px', boxSizing: 'border-box', resize: 'vertical'
        }}
      />

      <div style={{ marginBottom: '16px' }}>
        <label style={{ display: 'block', marginBottom: '6px', fontSize: '14px', color: '#4a5568' }}>
          Attach file {!note?.id && <span style={{ color: '#a0aec0' }}>(save note first)</span>}
        </label>
        <input type="file" onChange={handleUpload} disabled={uploading || !note?.id} />
        {uploadMsg && <p style={{ margin: '6px 0 0', fontSize: '13px' }}>{uploadMsg}</p>}
      </div>

      <div style={{ display: 'flex', gap: '10px' }}>
        <button
          onClick={handleSave}
          disabled={saving || !title.trim()}
          style={{
            background: '#4299e1', color: '#fff', border: 'none',
            padding: '10px 20px', borderRadius: '6px', cursor: 'pointer',
            fontSize: '14px', fontWeight: 600
          }}
        >
          {saving ? 'Saving...' : 'Save Note'}
        </button>
        <button
          onClick={onCancel}
          style={{
            background: '#edf2f7', color: '#4a5568', border: 'none',
            padding: '10px 20px', borderRadius: '6px', cursor: 'pointer', fontSize: '14px'
          }}
        >
          Cancel
        </button>
      </div>
    </div>
  );
}
