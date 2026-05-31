using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;
using SmartNotes.Api.Events;

namespace SmartNotes.Api.Services;

public class SqsService
{
    private readonly IAmazonSQS _sqs;
    private readonly string _queueUrl;

    public SqsService(IAmazonSQS sqs, IConfiguration config)
    {
        _sqs = sqs;
        _queueUrl = config["AWS:SQSQueueUrl"];
    }

    public async Task PublishAsync(Guid noteId, string action)
    {
        var message = new
        {
            NoteId = noteId,
            Action = action
        };

        await _sqs.SendMessageAsync(new SendMessageRequest
        {
            QueueUrl = _queueUrl,
            MessageBody = JsonSerializer.Serialize(message)
        });
    }
}