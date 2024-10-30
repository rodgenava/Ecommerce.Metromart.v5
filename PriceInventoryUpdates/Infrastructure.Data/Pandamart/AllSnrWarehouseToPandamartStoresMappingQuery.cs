using Data.Common.Contracts;
using Data.Projections;
using Data.Sql.Mapping;
using Data.Sql.Provider;
using Data.Sql;
using System.Data.Common;
using System.Data;
using Application;

namespace Infrastructure.Data
{
    public class AllSnrWarehouseToPandamartStoresMappingQuery : IAsyncQuery<IReadOnlyDictionary<Warehouse, PandamartStore>>
    {
        private readonly ISqlProvider _provider;
        private readonly ISqlCaller _caller;
        private readonly int _commandTimeout;

        public AllSnrWarehouseToPandamartStoresMappingQuery(string connectionString, int commandTimeout)
        {
            _caller = new SqlCaller(_provider = new SqlServerProvider(connectionString));
            _commandTimeout = commandTimeout;
        }
        async Task<IReadOnlyDictionary<Warehouse, PandamartStore>> IAsyncQuery<IReadOnlyDictionary<Warehouse, PandamartStore>>.ExecuteAsync(CancellationToken cancellationToken)
        {
            using DbCommand command = _provider.CreateCommand(
                commandString: "Select SnrWarehouseCode,SnrWarehouseDescription,MetromartStoreCode as PandamartStoreCode From MetromartSnrWarehouseMapping",
                commandType: CommandType.Text);

            command.CommandTimeout = _commandTimeout;

            IEnumerable<DataHolderPandaMart> items = await _caller.GetAsync(
                dataMapper: new ReflectionDataMapper<DataHolderPandaMart>(),
                command: command,
                cancellationToken: cancellationToken);

            return (from item in items
                    where !string.IsNullOrWhiteSpace(item.SnrWarehouseCode) && !string.IsNullOrWhiteSpace(item.SnrWarehouseDescription) && !string.IsNullOrWhiteSpace(item.PandamartStoreCode)
                    select item)
                    .ToDictionary(
                        keySelector: i => new Warehouse(
                            Code: int.Parse(i.SnrWarehouseCode),
                            Description: i.SnrWarehouseDescription),
                        elementSelector: i => new PandamartStore(Code: i.PandamartStoreCode));
        }
    }
}
