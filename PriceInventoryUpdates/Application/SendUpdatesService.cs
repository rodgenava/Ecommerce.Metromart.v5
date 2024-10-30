using Data.Common.Contracts;
using Data.Projections;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Application
{
    public class SendUpdatesService : ISendUpdatesService
    {
        private readonly IConfiguration _configuration;
        private readonly IAsyncQuery<MetromartPriceInventoryUpdate, Warehouse> _metromartPriceInventoryUpdateQuery;
        private readonly IAsyncQuery<IReadOnlyDictionary<Warehouse, MetromartStore>> _snrWarehouseMetromartMappingsQuery;
        private readonly IMetromartUpdateFileWriter _writer;
        private readonly IMetromartUpdatePathConventionFactory _metromartUpdatePathConventionFactory;
        private readonly IDispatchService _dispatchService;

        public SendUpdatesService(IConfiguration configuration, IAsyncQuery<MetromartPriceInventoryUpdate, Warehouse> metromartPriceInventoryUpdateQuery, IAsyncQuery<IReadOnlyDictionary<Warehouse, MetromartStore>> snrWarehouseMetromartMappingsQuery, IMetromartUpdateFileWriter writer, IMetromartUpdatePathConventionFactory metromartUpdatePathConventionFactory, IDispatchService dispatchService)
        {
            _configuration = configuration;
            _metromartPriceInventoryUpdateQuery = metromartPriceInventoryUpdateQuery;
            _snrWarehouseMetromartMappingsQuery = snrWarehouseMetromartMappingsQuery;
            _writer = writer;
            _metromartUpdatePathConventionFactory = metromartUpdatePathConventionFactory;
            _dispatchService = dispatchService;
        }

        //
        public async Task RunAsync(CancellationToken cancellationToken)
        {

            //GET Warehouses number equivalent to Metromart.
            //Get Warehouses code
            IReadOnlyDictionary<Warehouse, MetromartStore> warehouseMappings = await _snrWarehouseMetromartMappingsQuery.ExecuteAsync(cancellationToken: cancellationToken);

            if (!warehouseMappings.Any()) throw new InvalidOperationException(message: "Unable to create updates for metromart: No warehouse-metromart mappings has been configured.");

            IMetromartUpdatePathConvention metromartUpdatePathConvention = _metromartUpdatePathConventionFactory.Create();

            //Directory.GetParent(path)
            //using (var outputFileStream = new FileStream(path: "//FOR TESTING", mode: FileMode.Create))
            //{
            //    //save to path
            //    //await item.CopyToAsync(destination: outputFileStream, cancellationToken: cancellationToken);
            //}

            await Parallel.ForEachAsync(
                source: warehouseMappings,
                parallelOptions: new ParallelOptions()
                {
                    CancellationToken = cancellationToken,
                    MaxDegreeOfParallelism = _configuration.GetValue(
                        key: "Application:ParallelOperationPreference",
                        defaultValue: 1)
                },
                body: async (KeyValuePair<Warehouse, MetromartStore> KeyValuePair, CancellationToken ct) =>
                {
                    Warehouse warehouse = KeyValuePair.Key;
                    MetromartStore metromartStore = KeyValuePair.Value;

                    //Genarate Data to update
                    MetromartPriceInventoryUpdate update = await _metromartPriceInventoryUpdateQuery.ExecuteAsync(
                        parameter: warehouse,
                        cancellationToken: ct);

                    using(var memoryStream = new MemoryStream())
                    {
                        //write to memory
                        await _writer.WriteToStreamAsync(
                            updateStream: memoryStream,
                            items: update.Items,
                            cancellationToken: cancellationToken);

                        memoryStream.Seek(0, SeekOrigin.Begin);
                        
                        //Save to csv file
                        await _dispatchService.SendAsync( //SendAsync -> well send to DiskDispatchService to save the file in local path
                            item: memoryStream,
                            path: metromartUpdatePathConvention.Localize(warehouse: warehouse, metromartStore: metromartStore), //Localize -> Genarate file path and file name
                            cancellationToken: cancellationToken);
                    }
                });

        }
    }
}