using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using SmartNotes.Api.Data;
using Microsoft.EntityFrameworkCore;

public class NoteProcessorWorker : BackgroundService
{
    private readonly IAmazonSQS _sqs;
    private readonly IConfiguration _config;
    private readonly SmartNotesDbContext _db;

    public NoteProcessorWorker(
        IAmazonSQS sqs,
        IConfiguration config,
        SmartNotesDbContext db)
    {
        _sqs = sqs;
        _config = config;
        _db = db;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueUrl = _config["AWS:SQSQueueUrl"];

        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _sqs.ReceiveMessageAsync(new ReceiveMessageRequest
            {
                QueueUrl = queueUrl,
                MaxNumberOfMessages = 10,
                WaitTimeSeconds = 10
            }, stoppingToken);

            foreach (var message in response.Messages)
            {
                var payload = JsonSerializer.Deserialize<SqsMessage>(message.Body);

                var note = await _db.Notes.FirstOrDefaultAsync(n => n.Id == payload.NoteId);

                if (note != null)
                {
                    // TODO: embedding logic later
                    note.Embedding = new float[1536];

                    await _db.SaveChangesAsync();
                }

                await _sqs.DeleteMessageAsync(queueUrl, message.ReceiptHandle);
            }
        }
    }
}

public class SqsMessage
{
    public Guid NoteId { get; set; }
    public string Action { get; set; }
}