namespace SmartNotes.Api.Models;

public class Attachment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;

    public Guid NoteId { get; set; }
    public Note? Note { get; set; }
}