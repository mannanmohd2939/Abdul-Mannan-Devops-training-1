namespace SmartNotes.Api.DTOs;

public class NoteCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public List<string>? Tags { get; set; }

    public float[] Embedding { get; set; } = new float[1536];
}