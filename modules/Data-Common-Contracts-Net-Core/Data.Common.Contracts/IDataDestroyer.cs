using System.Threading;
using System.Threading.Tasks;

namespace Data.Common.Contracts
{
    public interface IDataDestroyer<T>
    {
        void Destroy(T item);
    }

    public interface IAsyncDataDestroyer<T>
    {
        Task Destroy(T item, CancellationToken cancellationToken = default);
    }
}
