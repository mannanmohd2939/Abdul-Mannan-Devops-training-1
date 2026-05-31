using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using SmartNotes.Api.Data;
using SmartNotes.Api.Models;
using Amazon.S3;
using Amazon.SQS;
using Amazon.Runtime;
using SmartNotes.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add DB
builder.Services.AddDbContext<SmartNotesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        options => options.UseVector()));

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS — allow React dev server
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// AWS — use real credentials if available, otherwise anonymous
var region = Amazon.RegionEndpoint.USEast1;
var hasAwsCreds =
    !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID")) ||
    System.IO.File.Exists(System.IO.Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".aws", "credentials"));

AWSCredentials awsCredentials = hasAwsCreds
    ? new Amazon.Runtime.CredentialManagement.CredentialProfileStoreChain().TryGetAWSCredentials("default", out var creds)
        ? creds
        : new AnonymousAWSCredentials()
    : new AnonymousAWSCredentials();

if (!hasAwsCreds)
    Console.WriteLine("[WARN] No AWS credentials found — S3/SQS will not work but app will run normally.");

builder.Services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client(awsCredentials, region));
builder.Services.AddSingleton<IAmazonSQS>(_ => new AmazonSQSClient(awsCredentials, region));

builder.Services.AddScoped<S3Service>();
builder.Services.AddScoped<SqsService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SmartNotesDbContext>();
    db.Database.Migrate();
    SeedTestData(db);
}

app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();

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
