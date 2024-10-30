using Application;

namespace PriceInventoryUpdatesConsoleApp
{
    public class DiskDispatchService : IDispatchService
    {
        public async Task SendAsync(Stream item, string path, CancellationToken cancellationToken)
        {
            Directory.GetParent(path)?.Create();

            using (var outputFileStream = new FileStream(path: path, mode: FileMode.Create))
            {
                //save to path
                await item.CopyToAsync(destination: outputFileStream, cancellationToken: cancellationToken);
            }
        }
    }
}
