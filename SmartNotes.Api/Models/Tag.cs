namespace SmartNotes.Api.Models;

public class Tag
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;

    public Guid NoteId { get; set; }
    public Note? Note { get; set; }
}