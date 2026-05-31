namespace SmartNotes.Api.Events;

public class NoteEvent
{
    public Guid NoteId { get; set; }
    public string Action { get; set; } = ""; // Created / Updated
}