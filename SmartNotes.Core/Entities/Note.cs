namespace SmartNotes.Core.Entities;

public class Note
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public float[] Embedding { get; set; } = new float[1536];
    public DateTime? EmbeddingGeneratedAt { get; set; }
    public List<Attachment> Attachments { get; set; } = new();
    public List<Tag> Tags { get; set; } = new();
}