using Minio;
using WebApp.Interfaces;
using Minio.DataModel.Args;

namespace WebApp.Services;

public class MinioStorageService : IFileStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;

    public MinioStorageService(IConfiguration configuration)
    {
        var minioConfig = configuration.GetSection("Minio");
        _minioClient = new MinioClient()
            .WithEndpoint(minioConfig["Endpoint"])
            .WithCredentials(minioConfig["AccessKey"], minioConfig["SecretKey"])
            .WithSSL(minioConfig.GetValue<bool>("UserSSL"))
            .Build();

        _bucketName = minioConfig["BucketName"];
        InitializeBucketAsync().Wait();
    }

    private async Task InitializeBucketAsync()
    {
        var exist = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName));
        if (!exist)
        {
            await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucketName));
        }
    }

    public async Task<string> UploadFileAsync(string fileName, string contentType, Stream fileStream)
    {
        var objectName = $"{Guid.NewGuid()}_{fileName}";

        await _minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(contentType));

        return objectName;
    }

    public async Task<Stream> GetFileAsync(string objectName)
    {
        var memoryStream = new MemoryStream();

        await _minioClient.GetObjectAsync(new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithCallbackStream(stream => stream.CopyTo(memoryStream))); // callback будет вызван, когда MinIO начнет возвращать данные

        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task DeleteFileAsync(string objectName)
    {
        await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName));
    }
}
