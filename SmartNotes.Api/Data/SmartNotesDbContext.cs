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
    }
}