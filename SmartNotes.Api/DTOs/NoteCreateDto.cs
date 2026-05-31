namespace SmartNotes.Api.DTOs;

public class NoteCreateDto
{
    public string Title { get; set; }
    public string Content { get; set; }
    public List<string>? Tags { get; set; }
}