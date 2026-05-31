using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;
using SmartNotes.Api.Events;

namespace SmartNotes.Api.Services;

public class SqsService
{
    private readonly IAmazonSQS _sqs;
    private readonly string? _queueUrl;
    private readonly ILogger<SqsService> _logger;

    public SqsService(IAmazonSQS sqs, IConfiguration config, ILogger<SqsService> logger)
    {
        _sqs = sqs;
        _queueUrl = config["AWS:SQSQueueUrl"];
        _logger = logger;
    }

    public async Task PublishAsync(Guid noteId, string action)
    {
        if (string.IsNullOrEmpty(_queueUrl))
        {
            _logger.LogWarning("SQS queue URL not configured — skipping message publish for NoteId {NoteId}", noteId);
            return;
        }

        try
        {
            var message = new { NoteId = noteId, Action = action };
            await _sqs.SendMessageAsync(new SendMessageRequest
            {
                QueueUrl = _queueUrl,
                MessageBody = JsonSerializer.Serialize(message)
            });
            _logger.LogInformation("Published {Action} message for NoteId {NoteId}", action, noteId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to publish SQS message for NoteId {NoteId} — continuing without async processing", noteId);
        }
    }
}
