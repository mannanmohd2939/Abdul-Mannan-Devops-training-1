namespace SmartNotes.Api.DTOs;

public class NoteDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = new();
}