using Application;
using Data.Common.Contracts;
using Data.Projections;
using Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Application.Enum;

namespace PriceInventoryUpdatesConsoleApp
{
    public class SendUpdatesServiceFactory : ISendUpdatesServiceFactory
    {
        private readonly IServiceProvider _provider;

        public SendUpdatesServiceFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public ISendUpdatesService CreateSendUpdatesService(UpdateType updateType)
        {
            IAsyncQuery < MetromartPriceInventoryUpdate, Warehouse > query = updateType switch
            {
                UpdateType.Full => _provider.GetRequiredService<MetromartPriceInventoryUpdateByWarehouseQuery>(),
                UpdateType.ConsignmentOnly => _provider.GetRequiredService<ConsignmentMetromartPriceInventoryUpdateByWarehouseQuery>(),
                _ => throw new Exception("Unreachable code block has somehow been reached.")
            };

            return new SendUpdatesService(
                configuration: _provider.GetRequiredService<IConfiguration>(),
                metromartPriceInventoryUpdateQuery: query,
                snrWarehouseMetromartMappingsQuery: _provider.GetRequiredService<IAsyncQuery<IReadOnlyDictionary<Warehouse, MetromartStore>>>(),
                writer: _provider.GetRequiredService<IMetromartUpdateFileWriter>(),
                metromartUpdatePathConventionFactory: _provider.GetRequiredService<IMetromartUpdatePathConventionFactory>(),
                dispatchService: _provider.GetRequiredService<IDispatchService>());
        }
    }
}
