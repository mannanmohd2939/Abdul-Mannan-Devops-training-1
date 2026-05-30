using Microsoft.EntityFrameworkCore;
using Pgvector;
using SmartNotes.Api.Models;

namespace SmartNotes.Api.Data;

public class SmartNotesDbContext : DbContext
{
    public SmartNotesDbContext(DbContextOptions<SmartNotesDbContext> options)
        : base(options)
    {
    }

    public DbSet<Note> Notes => Set<Note>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<Tag> Tags => Set<Tag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // IMPORTANT: enable pgvector extension
        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.Entity<Note>()
            .Property(x => x.Embedding)
            .HasConversion(
                v => new Vector(v),
                v => v.ToArray())
            .HasColumnType("vector(1536)");

        modelBuilder.Entity<Note>()
            .HasMany(n => n.Attachments)
            .WithOne(a => a.Note)
            .HasForeignKey(a => a.NoteId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Note>()
            .HasMany(n => n.Tags)
            .WithOne(t => t.Note)
            .HasForeignKey(t => t.NoteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}