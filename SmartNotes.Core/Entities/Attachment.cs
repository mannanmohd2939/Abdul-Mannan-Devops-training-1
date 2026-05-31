namespace SmartNotes.Core.Entities;

public class Attachment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid NoteId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string S3Key { get; set; } = string.Empty;
    public Note Note { get; set; } = null!;
}