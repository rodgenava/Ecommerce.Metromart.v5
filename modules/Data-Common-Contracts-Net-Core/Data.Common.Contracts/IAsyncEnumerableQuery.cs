using System.Collections.Generic;
using System.Threading;

namespace Data.Common.Contracts
{
    public interface IAsyncEnumerableQuery<TReturn>
    {
        IAsyncEnumerable<TReturn> ExecuteAsync(CancellationToken cancellationToken = default);
    }

    public interface IAsyncEnumerableQuery<TReturn, in TParameter>
    {
        IAsyncEnumerable<TReturn> ExecuteAsync(TParameter parameter, CancellationToken cancellationToken = default);
    }

    public interface IAsyncEnumerableQuery<TReturn, in TParameter1, in TParameter2>
    {
        IAsyncEnumerable<TReturn> ExecuteAsync(TParameter1 parameter1, TParameter2 parameter2, CancellationToken cancellationToken = default);
    }

    public interface IAsyncEnumerableQuery<TReturn, in TParameter1, in TParameter2, in TParameter3>
    {
        IAsyncEnumerable<TReturn> ExecuteAsync(TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3, CancellationToken cancellationToken = default);
    }

    public interface IAsyncEnumerableQuery<TReturn, in TParameter1, in TParameter2, in TParameter3, in TParameter4>
    {
        IAsyncEnumerable<TReturn> ExecuteAsync(TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3, TParameter4 parameter4, CancellationToken cancellationToken = default);
    }

    public interface IAsyncEnumerableQuery<TReturn, in TParameter1, in TParameter2, in TParameter3, in TParameter4, in TParameter5>
    {
        IAsyncEnumerable<TReturn> ExecuteAsync(TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3, TParameter4 parameter4, TParameter5 parameter5, CancellationToken cancellationToken = default);
    }

    public interface IAsyncEnumerableQuery<TReturn, in TParameter1, in TParameter2, in TParameter3, in TParameter4, in TParameter5, in TParameter6>
    {
        IAsyncEnumerable<TReturn> ExecuteAsync(TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3, TParameter4 parameter4, TParameter5 parameter5, TParameter6 parameter6, CancellationToken cancellationToken = default);
    }

    public interface IAsyncEnumerableQuery<TReturn, in TParameter1, in TParameter2, in TParameter3, in TParameter4, in TParameter5, in TParameter6, in TParameter7>
    {
        IAsyncEnumerable<TReturn> ExecuteAsync(TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3, TParameter4 parameter4, TParameter5 parameter5, TParameter6 parameter6, TParameter7 parameter7, CancellationToken cancellationToken = default);
    }

    public interface IAsyncEnumerableQuery<TReturn, in TParameter1, in TParameter2, in TParameter3, in TParameter4, in TParameter5, in TParameter6, in TParameter7, in TParameter8>
    {
        IAsyncEnumerable<TReturn> ExecuteAsync(TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3, TParameter4 parameter4, TParameter5 parameter5, TParameter6 parameter6, TParameter7 parameter7, TParameter8 parameter8, CancellationToken cancellationToken = default);
    }
}
