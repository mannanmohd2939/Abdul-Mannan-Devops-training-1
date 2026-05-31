using Microsoft.EntityFrameworkCore;
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
            .HasConversion(
                v => new Vector(v),
                v => v.ToArray())
            .HasColumnType("vector(1536)");
    }
}