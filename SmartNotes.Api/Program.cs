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
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<Amazon.S3.IAmazonS3>();
builder.Services.AddScoped<S3Service>();
builder.Services.AddAWSService<IAmazonSQS>();
builder.Services.AddAWSService<Amazon.SQS.IAmazonSQS>();
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

app.MapControllers();

app.Run();

static void SeedTestData(SmartNotesDbContext db)
{
    if (db.Notes.Any())
    {
        return;
    }

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

    note.Tags.Add(new Tag
    {
        Name = "seed"
    });

    db.Notes.Add(note);
    db.SaveChanges();
}