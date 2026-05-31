using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using SmartNotes.Worker.Data;
using Amazon.SQS;
using Amazon.SimpleNotificationService;

var builder = Host.CreateApplicationBuilder(args);

// --------------------
// DB Context
// --------------------
builder.Services.AddDbContext<SmartNotesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// --------------------
// AWS Services
// --------------------
builder.Services.AddAWSService<IAmazonSQS>();
builder.Services.AddAWSService<IAmazonSimpleNotificationService>();

// --------------------
// Worker Service
// --------------------
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();