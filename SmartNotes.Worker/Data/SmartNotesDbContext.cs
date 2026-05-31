using Microsoft.EntityFrameworkCore;
using Pgvector;
using SmartNotes.Core.Entities;

namespace SmartNotes.Worker.Data;

public class SmartNotesDbContext : DbContext
{
    public SmartNotesDbContext(DbContextOptions<SmartNotesDbContext> options)
        : base(options) { }

    public DbSet<Note> Notes => Set<Note>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.Entity<Note>()
            .Property(x => x.Embedding)
            .HasColumnType("vector(1536)");

        modelBuilder.Entity<Attachment>()
            .HasOne(a => a.Note)
            .WithMany(n => n.Attachments)
            .HasForeignKey(a => a.NoteId);

        modelBuilder.Entity<Tag>()
            .HasOne(t => t.Note)
            .WithMany(n => n.Tags)
            .HasForeignKey(t => t.NoteId);
    }
}