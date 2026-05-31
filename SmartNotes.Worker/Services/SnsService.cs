using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

namespace SmartNotes.Worker.Services;

public class SnsService
{
    private readonly IAmazonSimpleNotificationService _sns;
    private readonly string _topicArn;

    public SnsService(IAmazonSimpleNotificationService sns, IConfiguration config)
    {
        _sns = sns;
        _topicArn = config["AWS:SnsTopicArn"]!;
    }

    public async Task PublishAsync(string message)
    {
        await _sns.PublishAsync(new PublishRequest
        {
            TopicArn = _topicArn,
            Message = message
        });
    }
}