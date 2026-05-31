import axios from 'axios';

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000/api',
});

export const getNotes = () => api.get('/notes');
export const getNote = (id) => api.get(`/notes/${id}`);
export const createNote = (data) => api.post('/notes', data);
export const updateNote = (id, data) => api.put(`/notes/${id}`, data);
export const deleteNote = (id) => api.delete(`/notes/${id}`);
export const searchNotes = (query) => api.get(`/notes/search?q=${encodeURIComponent(query)}`);
export const uploadFile = (noteId, file) => {
  const form = new FormData();
  form.append('file', file);
  return api.post(`/notes/${noteId}/upload`, form);
};

export default api;
