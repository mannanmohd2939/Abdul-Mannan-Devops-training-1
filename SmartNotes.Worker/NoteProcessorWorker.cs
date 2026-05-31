using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using SmartNotes.Worker.Data;

namespace SmartNotes.Worker;

public class NoteProcessorWorker : BackgroundService
{
    private readonly IAmazonSQS _sqs;
    private readonly IConfiguration _config;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<NoteProcessorWorker> _logger;

    public NoteProcessorWorker(
        IAmazonSQS sqs,
        IConfiguration config,
        IServiceScopeFactory scopeFactory,
        ILogger<NoteProcessorWorker> logger)
    {
        _sqs = sqs;
        _config = config;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queueUrl = _config["Aws:SqsQueueUrl"];
        _logger.LogInformation("Worker started, polling: {QueueUrl}", queueUrl);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var response = await _sqs.ReceiveMessageAsync(new ReceiveMessageRequest
                {
                    QueueUrl = queueUrl,
                    MaxNumberOfMessages = 10,
                    WaitTimeSeconds = 20
                }, stoppingToken);

                foreach (var message in response.Messages)
                {
                    await ProcessMessageAsync(message, queueUrl, stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in polling loop, retrying in 5s");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task ProcessMessageAsync(Message message, string? queueUrl, CancellationToken ct)
    {
        try
        {
            var payload = JsonSerializer.Deserialize<SqsMessage>(message.Body);
            if (payload is null)
            {
                _logger.LogWarning("Could not deserialize message, dropping it");
                await _sqs.DeleteMessageAsync(queueUrl, message.ReceiptHandle, ct);
                return;
            }

            _logger.LogInformation("Processing NoteId {NoteId}", payload.NoteId);

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SmartNotesDbContext>();

            var note = await db.Notes.FirstOrDefaultAsync(n => n.Id == payload.NoteId, ct);
            if (note is not null)
            {
                // Placeholder: replace with real OpenAI embedding call later
                note.Embedding = new float[1536];
                note.EmbeddingGeneratedAt = DateTime.UtcNow;
                await db.SaveChangesAsync(ct);
                _logger.LogInformation("Updated embedding for NoteId {NoteId}", payload.NoteId);
            }
            else
            {
                _logger.LogWarning("NoteId {NoteId} not found in DB", payload.NoteId);
            }

            await _sqs.DeleteMessageAsync(queueUrl, message.ReceiptHandle, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process message {MessageId} — will retry", message.MessageId);
            // Don't delete — SQS will redeliver after visibility timeout
        }
    }
}

public class SqsMessage
{
    public Guid NoteId { get; set; }
    public string Action { get; set; } = string.Empty;
}