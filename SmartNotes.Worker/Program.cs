using Microsoft.EntityFrameworkCore;
using SmartNotes.Worker;
using SmartNotes.Worker.Data;
using Amazon.SQS;
using Amazon.SimpleNotificationService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<SmartNotesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAWSService<IAmazonSQS>();
builder.Services.AddAWSService<IAmazonSimpleNotificationService>();

builder.Services.AddHostedService<NoteProcessorWorker>();

var host = builder.Build();
host.Run();