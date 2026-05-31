using Amazon.SQS;
using Amazon.SQS.Model;

namespace SmartNotes.Worker.Services;

public class SqsListenerService
{
    private readonly IAmazonSQS _sqs;
    private readonly string _queueUrl;

    public SqsListenerService(IAmazonSQS sqs, IConfiguration config)
    {
        _sqs = sqs;
        _queueUrl = config["AWS:SqsQueueUrl"]!;
    }

    public async Task<List<string>> GetMessagesAsync()
    {
        var response = await _sqs.ReceiveMessageAsync(new ReceiveMessageRequest
        {
            QueueUrl = _queueUrl,
            MaxNumberOfMessages = 5,
            WaitTimeSeconds = 10
        });

        return response.Messages.Select(m => m.Body).ToList();
    }

    public async Task DeleteMessageAsync(string receiptHandle)
    {
        await _sqs.DeleteMessageAsync(_queueUrl, receiptHandle);
    }
}