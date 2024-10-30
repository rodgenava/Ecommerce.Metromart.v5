using Data.Common.Contracts;
using Data.Projections;
using Data.Sql.Provider;
using Data.Sql;
using System.Data.Common;
using System.Data;
using Data.Sql.Mapping;
using Application;

namespace Infrastructure.Data
{
    public class AllSnrWarehouseToMetromartStoresMappingQuery : IAsyncQuery<IReadOnlyDictionary<Warehouse, MetromartStore>>
    {
        private readonly ISqlProvider _provider;
        private readonly ISqlCaller _caller;
        private readonly int _commandTimeout;

        public AllSnrWarehouseToMetromartStoresMappingQuery(string connectionString, int commandTimeout)
        {
            _caller = new SqlCaller(_provider = new SqlServerProvider(connectionString));
            _commandTimeout = commandTimeout;
        }
        async Task<IReadOnlyDictionary<Warehouse, MetromartStore>> IAsyncQuery<IReadOnlyDictionary<Warehouse, MetromartStore>>.ExecuteAsync(CancellationToken cancellationToken)
        {
            using DbCommand command = _provider.CreateCommand(
                commandString: "Select SnrWarehouseCode,SnrWarehouseDescription,MetromartStoreCode From MetromartSnrWarehouseMapping",
                commandType: CommandType.Text);

            command.CommandTimeout = _commandTimeout;

            IEnumerable<DataHolder> items = await _caller.GetAsync(
                dataMapper: new ReflectionDataMapper<DataHolder>(),
                command: command,
                cancellationToken: cancellationToken);

            return (from item in items
                    where !string.IsNullOrWhiteSpace(item.SnrWarehouseCode) && !string.IsNullOrWhiteSpace(item.SnrWarehouseDescription) && !string.IsNullOrWhiteSpace(item.MetromartStoreCode)
                    select item)
                    .ToDictionary(
                        keySelector: i => new Warehouse(
                            Code: int.Parse(i.SnrWarehouseCode), 
                            Description: i.SnrWarehouseDescription), 
                        elementSelector: i => new MetromartStore(Code: i.MetromartStoreCode));
        }
    }
}