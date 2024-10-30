using System.Threading;
using System.Threading.Tasks;

namespace Data.Common.Contracts
{
    public interface IRepository<TKey, TItem>
    {
        TItem? Find(TKey key);
        void Save(TItem item);
    }

    public interface IAsyncRepository<TKey, TItem>
    {
        Task<TItem?> FindAsync(TKey key, CancellationToken cancellationToken = default);
        Task SaveAsync(TItem item, CancellationToken cancellationToken = default);
    }

    //Template for specific implementations: SqlSpec(string ToSqlClase()), GenericSpec(bool IsSatisfiedBy(condition))... etc.
    public interface ISpecification
    {

    }

    public interface IRepository<TItem>
    {
        TItem? Find(ISpecification specs);
        void Save(TItem item);
    }

    public interface IAsyncRepository<TItem>
    {
        Task<TItem?> FindAsync(ISpecification specs, CancellationToken cancellationToken = default);
        Task SaveAsync(TItem item, CancellationToken cancellationToken = default);
    }
}
