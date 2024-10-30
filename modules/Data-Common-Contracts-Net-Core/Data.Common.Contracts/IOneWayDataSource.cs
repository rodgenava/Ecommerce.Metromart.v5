using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Common.Contracts
{
    public interface IOneWayDataSource<T>
    {
        IEnumerable<T> Next();
    }

    public interface IAsyncOneWayDataSource<T>
    {
        Task<IEnumerable<T>> NextAsync(CancellationToken cancellationToken = default);
    }
}
