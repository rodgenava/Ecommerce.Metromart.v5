using System;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Common.Contracts
{
    public interface IDataSourceIterator<TItem>
    {
        void Iterate(Action<TItem> itemCallback);
    }

    public interface IDataSourceIterator<TItem, TParameter>
    {
        void Iterate(Action<TItem> itemCallback, TParameter parameter);
    }

    public interface IAsyncDataSourceIterator<TItem>
    {
        Task IterateAsync(Action<TItem> itemCallback, CancellationToken cancellationToken = default);
    }

    public interface IAsyncDataSourceIterator<TItem, TParameter>
    {
        Task IterateAsync(Action<TItem> itemCallback, TParameter parameter, CancellationToken cancellationToken = default);
    }
}
