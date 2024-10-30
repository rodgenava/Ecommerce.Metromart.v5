using System.Threading;
using System.Threading.Tasks;

namespace Data.Common.Contracts
{
    public interface IDataWriter<TData>
    {
        void Write(TData data);

    }

    public interface IDataWriter<TData, TGeneratedId>
    {
        TGeneratedId Write(TData data);

    }

    public interface IAsyncDataWriter<TData>
    {
        Task WriteAsync(TData data, CancellationToken cancellationToken = default);

    }

    public interface IAsyncDataWriter<TData, TGeneratedId>
    {
        Task<TGeneratedId> WriteAsync(TData data, CancellationToken cancellationToken = default);

    }
}
