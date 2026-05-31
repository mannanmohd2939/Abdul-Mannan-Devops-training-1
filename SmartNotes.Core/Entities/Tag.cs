namespace SmartNotes.Core.Entities;

public class Tag
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid NoteId { get; set; }
    public string Name { get; set; } = string.Empty;
    public Note Note { get; set; } = null!;
}