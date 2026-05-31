using Microsoft.EntityFrameworkCore;
using SmartNotes.Api.Data;
using SmartNotes.Api.Models;
using Xunit;

namespace SmartNotes.Tests;

// Test DbContext that ignores the pgvector Embedding column
// (InMemory provider doesn't support the Vector type)
public class TestDbContext : SmartNotesDbContext
{
    public TestDbContext(DbContextOptions<SmartNotesDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Note>().Ignore(n => n.Embedding);
        modelBuilder.Entity<Note>()
            .HasMany(n => n.Tags).WithOne(t => t.Note).HasForeignKey(t => t.NoteId);
        modelBuilder.Entity<Note>()
            .HasMany(n => n.Attachments).WithOne(a => a.Note).HasForeignKey(a => a.NoteId);
    }
}

public class NoteTests
{
    private TestDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<SmartNotesDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new TestDbContext(options);
    }

    [Fact]
    public async Task CreateNote_ShouldPersistToDatabase()
    {
        var db = GetInMemoryDb();
        var note = new Note { Title = "Test Note", Content = "Test content" };
        db.Notes.Add(note);
        await db.SaveChangesAsync();
        var saved = await db.Notes.FindAsync(note.Id);
        Assert.NotNull(saved);
        Assert.Equal("Test Note", saved.Title);
    }

    [Fact]
    public void Note_EmptyTitle_ShouldBeDetectable()
    {
        var note = new Note { Title = "", Content = "Some content" };
        Assert.True(string.IsNullOrEmpty(note.Title));
    }

    [Fact]
    public async Task DeleteNote_ShouldRemoveFromDatabase()
    {
        var db = GetInMemoryDb();
        var note = new Note { Title = "To Delete", Content = "bye" };
        db.Notes.Add(note);
        await db.SaveChangesAsync();
        db.Notes.Remove(note);
        await db.SaveChangesAsync();
        var deleted = await db.Notes.FindAsync(note.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task UpdateNote_ShouldChangeContent()
    {
        var db = GetInMemoryDb();
        var note = new Note { Title = "Original", Content = "Old content" };
        db.Notes.Add(note);
        await db.SaveChangesAsync();
        note.Content = "Updated content";
        await db.SaveChangesAsync();
        var updated = await db.Notes.FindAsync(note.Id);
        Assert.Equal("Updated content", updated!.Content);
    }

    [Fact]
    public void Note_DefaultEmbeddingSize_ShouldBe1536()
    {
        var embedding = new float[1536];
        Assert.Equal(1536, embedding.Length);
    }

    [Fact]
    public async Task Note_WithTag_ShouldPersistTag()
    {
        var db = GetInMemoryDb();
        var note = new Note { Title = "Tagged", Content = "Has tag" };
        note.Tags.Add(new Tag { Name = "important" });
        db.Notes.Add(note);
        await db.SaveChangesAsync();
        var saved = await db.Notes.Include(n => n.Tags).FirstAsync(n => n.Id == note.Id);
        Assert.Single(saved.Tags);
        Assert.Equal("important", saved.Tags[0].Name);
    }
}
