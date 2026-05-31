using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNotes.Api.Data;
using SmartNotes.Api.Models;
using SmartNotes.Api.DTOs;

namespace SmartNotes.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly SmartNotesDbContext _context;

    public NotesController(SmartNotesDbContext context)
    {
        _context = context;
    }

    // GET ALL NOTES
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var notes = await _context.Notes
            .Include(n => n.Tags)
            .ToListAsync();

        var result = notes.Select(n => new NoteDto
        {
            Id = n.Id,
            Title = n.Title,
            Content = n.Content,
            Tags = n.Tags.Select(t => t.Name).ToList()
       });

       return Ok(result);
    }

    // GET ONE NOTE
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var note = await _context.Notes
            .Include(n => n.Tags)
            .FirstOrDefaultAsync(n => n.Id == id);

        if (note == null) return NotFound();

        var result = new NoteDto
       {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            Tags = note.Tags.Select(t => t.Name).ToList()
        };

        return Ok(result);
    }

    // CREATE NOTE
    [HttpPost]
    public async Task<IActionResult> Create(NoteCreateDto dto)
    {
        var note = new Note
        { 
            Title = dto.Title,
            Content = dto.Content,
            Embedding = null! // will be generated later
        };

        if (dto.Tags != null)
       {
            note.Tags = dto.Tags.Select(t => new Tag
           {
                Name = t
           }).ToList();
       }

        _context.Notes.Add(note);
        await _context.SaveChangesAsync();

        return Ok(note.Id);
    }

    // UPDATE NOTE
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, Note input)
    {
        var note = await _context.Notes.FindAsync(id);
        if (note == null) return NotFound();

        note.Title = input.Title;
        note.Content = input.Content;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE NOTE
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var note = await _context.Notes.FindAsync(id);
        if (note == null) return NotFound();

        _context.Notes.Remove(note);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}