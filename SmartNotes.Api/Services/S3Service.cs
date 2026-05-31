using Amazon.S3;
using Amazon.S3.Model;

namespace SmartNotes.Api.Services;

public class S3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public S3Service(IConfiguration config, IAmazonS3 s3Client)
    {
        _s3Client = s3Client;

        _bucketName = config["AWS:S3Bucket"]
            ?? throw new Exception("S3 Bucket is missing in appsettings.json");
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        var key = $"{Guid.NewGuid()}-{file.FileName}";

        using var stream = file.OpenReadStream();

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = stream,
            ContentType = file.ContentType
        };

        await _s3Client.PutObjectAsync(request);

        return $"https://{_bucketName}.s3.amazonaws.com/{key}";
    }
}