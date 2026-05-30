using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using SmartNotes.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add DB
builder.Services.AddDbContext<SmartNotesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        options => options.UseVector()));

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();