using Application;
using Data.Common.Contracts;
using Data.Projections;
using Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Application.Enum;

namespace PriceInventoryUpdatesConsoleApp
{
    public class SendPandaMartUpdatesServiceFactory : ISendPandaMartUpdatesServiceFactory
    {
        private readonly IServiceProvider _provider;

        public SendPandaMartUpdatesServiceFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public ISendPandaMartUpdatesService CreateSendPandaMartUpdatesService(UpdateTypepanda updateType)
        {
            IAsyncQueryPandamart<PandamartPriceInventoryUpdate, Warehouse> query = updateType switch
            {
                UpdateTypepanda.Full => _provider.GetRequiredService<PandamartPriceInventoryUpdateByWarehouseQuery>(),
                UpdateTypepanda.ConsignmentOnly => _provider.GetRequiredService<ConsignmentPandamartPriceInventoryUpdateByWarehouseQuery>(),
                _ => throw new Exception("Unreachable code block has somehow been reached.")
            };

            return new SendPandaMartUpdatesService(
                configuration: _provider.GetRequiredService<IConfiguration>(),
                pandamartPriceInventoryUpdateQuery: query,
                snrWarehousePandamartMappingsQuery: _provider.GetRequiredService<IAsyncQuery<IReadOnlyDictionary<Warehouse, PandamartStore>>>(),
                writer: _provider.GetRequiredService<IPandamartUpdateFileWriter>(),
                pandamartUpdatePathConventionFactory: _provider.GetRequiredService<IPandamartUpdatePathConventionFactory>(),
                dispatchService: _provider.GetRequiredService<IDispatchService>(),
                sftpPandamart: _provider.GetRequiredService<ISFTPPandamart>(),
                localDataPath: _provider.GetRequiredService<ILocalDataPath>());
        }


    }
}
