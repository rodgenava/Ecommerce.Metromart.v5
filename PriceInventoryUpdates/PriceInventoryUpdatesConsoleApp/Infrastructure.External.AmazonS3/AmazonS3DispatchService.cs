using Amazon.S3;
using Amazon.S3.Model;
using Application;
using Polly;

namespace PriceInventoryUpdatesConsoleApp.Infrastructure.External.AmazonS3
{
    public class AmazonS3DispatchService : IDispatchService
    {
        private readonly AmazonS3Client _client;
        private readonly string _bucketName;
        private readonly IAsyncPolicy _asyncPolicy;

        public AmazonS3DispatchService(AmazonS3Client client, string bucketName, IAsyncPolicy asyncPolicy)
        {
            _client = client;
            _bucketName = bucketName;
            _asyncPolicy = asyncPolicy;
        }

        public async Task SendAsync(Stream item, string path, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            PutObjectResponse _ = await _asyncPolicy.ExecuteAsync<PutObjectResponse>(async (CancellationToken ct) =>
            {
                using var stream = new MemoryStream();

                await item.CopyToAsync(stream, ct);

                return await _client.PutObjectAsync(
                    request: new PutObjectRequest()
                    {
                        BucketName = _bucketName,
                        Key = path,
                        InputStream = stream,
                        ContentType = "text/plain"
                    },
                    cancellationToken: ct);
            },
            cancellationToken: cancellationToken);
        }
    }
}
