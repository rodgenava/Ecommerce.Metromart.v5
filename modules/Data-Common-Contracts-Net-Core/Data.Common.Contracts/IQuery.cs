using System.Threading;
using System.Threading.Tasks;

namespace Data.Common.Contracts
{
    public interface IQuery<TReturn>
    {
        TReturn Execute();
    }

    public interface IQuery<TReturn, in TParameter>
    {
        TReturn Execute(TParameter parameter);
    }

    public interface IQuery<TReturn, in TParameter1, in TParameter2>
    {
        TReturn Execute(TParameter1 parameter1, TParameter2 parameter2);
    }

    public interface IQuery<TReturn, in TParameter1, in TParameter2, in TParameter3>
    {
        TReturn Execute(TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3);
    }

    public interface IQuery<TReturn, in TParameter1, in TParameter2, in TParameter3, in TParameter4>
    {
        TReturn Execute(TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3, TParameter4 parameter4);
    }

    public interface IQuery<TReturn, in TParameter1, in TParameter2, in TParameter3, in TParameter4, in TParameter5>
    {
        TReturn Execute(TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3, TParameter4 parameter4, TParameter5 parameter5);
    }

    public interface IQuery<TReturn, in TParameter1, in TParameter2, in TParameter3, in TParameter4, in TParameter5, in TParameter6>
    {
        TReturn Execute(TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3, TParameter4 parameter4, TParameter5 parameter5, TParameter6 parameter6);
    }

    public interface IQuery<TReturn, in TParameter1, in TParameter2, in TParameter3, in TParameter4, in TParameter5, in TParameter6, in TParameter7>
    {
        TReturn Execute(TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3, TParameter4 parameter4, TParameter5 parameter5, TParameter6 parameter6, TParameter7 parameter7);
    }

    public interface IQuery<TReturn, in TParameter1, in TParameter2, in TParameter3, in TParameter4, in TParameter5, in TParameter6, in TParameter7, in TParameter8>
    {
        TReturn Execute(TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3, TParameter4 parameter4, TParameter5 parameter5, TParameter6 parameter6, TParameter7 parameter7, TParameter8 parameter8);
    }

    public interface IAsyncQuery<TReturn>
    {
        Task<TReturn> ExecuteAsync(CancellationToken cancellationToken = default);
    }

    public interface IAsyncQuery<TReturn, TParameter>
    {
        Task<TReturn> ExecuteAsync(TParameter parameter, CancellationToken cancellationToken = default);
    }

    public interface IAsyncQuery<TReturn, in TParameter1, in TParameter2>
    {
        Task<TReturn> ExecuteAsync(TParameter1 parameter1, TParameter2 parameter2, CancellationToken cancellationToken = default);
    }

    public interface IAsyncQuery<TReturn, in TParameter1, in TParameter2, in TParameter3>
    {
        Task<TReturn> ExecuteAsync(TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3, CancellationToken cancellationToken = default);
    }

    public interface IAsyncQuery<TReturn, in TParameter1, in TParameter2, in TParameter3, in TParameter4>
    {
        Task<TReturn> ExecuteAsync(TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3, TParameter4 parameter4, CancellationToken cancellationToken = default);
    }

    public interface IAsyncQuery<TReturn, in TParameter1, in TParameter2, in TParameter3, in TParameter4, in TParameter5>
    {
        Task<TReturn> ExecuteAsync(TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3, TParameter4 parameter4, TParameter5 parameter5, CancellationToken cancellationToken = default);
    }

    public interface IAsyncQuery<TReturn, in TParameter1, in TParameter2, in TParameter3, in TParameter4, in TParameter5, in TParameter6>
    {
        Task<TReturn> ExecuteAsync(TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3, TParameter4 parameter4, TParameter5 parameter5, TParameter6 parameter6, CancellationToken cancellationToken = default);
    }

    public interface IAsyncQuery<TReturn, in TParameter1, in TParameter2, in TParameter3, in TParameter4, in TParameter5, in TParameter6, in TParameter7>
    {
        Task<TReturn> ExecuteAsync(TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3, TParameter4 parameter4, TParameter5 parameter5, TParameter6 parameter6, TParameter7 parameter7, CancellationToken cancellationToken = default);
    }
    
    public interface IAsyncQuery<TReturn, in TParameter1, in TParameter2, in TParameter3, in TParameter4, in TParameter5, in TParameter6, in TParameter7, in TParameter8>
    {
        Task<TReturn> ExecuteAsync(TParameter1 parameter1, TParameter2 parameter2, TParameter3 parameter3, TParameter4 parameter4, TParameter5 parameter5, TParameter6 parameter6, TParameter7 parameter7, TParameter8 parameter8, CancellationToken cancellationToken = default);
    }

    //PandaMart

    public interface IAsyncQueryPandamart<TReturn, TParameter>
    {
        Task<TReturn> ExecuteAsyncPandamart(TParameter parameter, CancellationToken cancellationToken = default);
    }


}
