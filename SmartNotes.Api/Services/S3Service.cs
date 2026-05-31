using Amazon.S3;
using Amazon.S3.Model;

namespace SmartNotes.Api.Services;

public class S3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly string? _bucketName;
    private readonly ILogger<S3Service> _logger;

    public S3Service(IConfiguration config, IAmazonS3 s3Client, ILogger<S3Service> logger)
    {
        _s3Client = s3Client;
        _bucketName = config["AWS:S3Bucket"];
        _logger = logger;
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        if (string.IsNullOrEmpty(_bucketName))
        {
            _logger.LogWarning("S3 bucket not configured — skipping upload for {FileName}", file.FileName);
            return $"local://{file.FileName}";
        }

        try
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
            var url = $"https://{_bucketName}.s3.amazonaws.com/{key}";
            _logger.LogInformation("Uploaded {FileName} to S3 at {Url}", file.FileName, url);
            return url;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to upload {FileName} to S3 — returning placeholder URL", file.FileName);
            return $"local://{file.FileName}";
        }
    }
}
