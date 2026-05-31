using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using SmartNotes.Api.Data;
using SmartNotes.Api.Models;
using Amazon.S3;
using SmartNotes.Api.Services;
using Amazon.SQS;

var builder = WebApplication.CreateBuilder(args);

// Add DB
builder.Services.AddDbContext<SmartNotesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        options => options.UseVector()));

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AWS — register but don't crash if credentials missing
try
{
    builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
    builder.Services.AddAWSService<IAmazonS3>();
    builder.Services.AddAWSService<IAmazonSQS>();
}
catch (Exception ex)
{
    Console.WriteLine($"[WARN] AWS services could not be configured: {ex.Message}");
    // Register no-op stubs so DI doesn't fail
    builder.Services.AddSingleton<IAmazonS3>(_ =>
        new AmazonS3Client(new Amazon.Runtime.AnonymousAWSCredentials(), Amazon.RegionEndpoint.USEast1));
    builder.Services.AddSingleton<IAmazonSQS>(_ =>
        new AmazonSQSClient(new Amazon.Runtime.AnonymousAWSCredentials(), Amazon.RegionEndpoint.USEast1));
}

builder.Services.AddScoped<S3Service>();
builder.Services.AddScoped<SqsService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SmartNotesDbContext>();
    db.Database.Migrate();
    SeedTestData(db);
}

app.UseSwagger();
app.UseSwaggerUI();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.MapControllers();
app.Run();

static void SeedTestData(SmartNotesDbContext db)
{
    if (db.Notes.Any()) return;

    var note = new Note
    {
        Title = "Welcome to SmartNotes",
        Content = "This note was seeded with an embedding, attachment, and tag.",
        Embedding = new float[1536]
    };
    note.Attachments.Add(new Attachment
    {
        FileName = "welcome.txt",
        Url = "https://example.com/welcome.txt"
    });
    note.Tags.Add(new Tag { Name = "seed" });
    db.Notes.Add(note);
    db.SaveChanges();
}
