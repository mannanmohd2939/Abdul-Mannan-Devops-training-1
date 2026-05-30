using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNotes.Api.Data;
using SmartNotes.Api.Models;
using SmartNotes.Api.DTOs;
using Pgvector;

namespace SmartNotes.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly SmartNotesDbContext _db;

    public NotesController(SmartNotesDbContext db)
    {
        _db = db;
    }

    // CREATE
    [HttpPost]
    public async Task<IActionResult> Create(NoteCreateDto dto)
   {
        var note = new Note
        {
            Title = dto.Title,
            Content = dto.Content,
            Embedding = dto.Embedding
        };

        _db.Notes.Add(note);
        await _db.SaveChangesAsync();

        return Ok(note);
    }

    // GET ALL
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _db.Notes.ToListAsync());
    }

    // GET BY ID
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var note = await _db.Notes.FindAsync(id);
        return note == null ? NotFound() : Ok(note);
    }

    // DELETE
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var note = await _db.Notes.FindAsync(id);
        if (note == null) return NotFound();

        _db.Notes.Remove(note);
        await _db.SaveChangesAsync();
        return Ok();
    }

    // SEMANTIC SEARCH (basic version)
    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] float[] query)
   {
        var vector = new Vector(query);

        var results = await _db.Notes
            .FromSqlRaw(@"
                SELECT * FROM ""Notes""
                ORDER BY ""Embedding"" <-> {0}
                LIMIT 5
            ", vector)
            .ToListAsync();

        return Ok(results);
    }
}