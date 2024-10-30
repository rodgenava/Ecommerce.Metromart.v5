using Data.Common.Contracts;
using Data.Projections;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class SendPandaMartUpdatesService : ISendPandaMartUpdatesService
    {
        private readonly IConfiguration _configuration;
        private readonly IAsyncQueryPandamart<PandamartPriceInventoryUpdate, Warehouse> _pandamartPriceInventoryUpdateQuery;
        private readonly IAsyncQuery<IReadOnlyDictionary<Warehouse, PandamartStore>> _snrWarehousePandamartMappingsQuery;
        private readonly IPandamartUpdateFileWriter _writer;
        private readonly IPandamartUpdatePathConventionFactory _pandamartUpdatePathConventionFactory;
        private readonly IDispatchService _dispatchService;
        private readonly ISFTPPandamart _sftpPandamart;
        private readonly ILocalDataPath _localDataPath;

        public SendPandaMartUpdatesService(IConfiguration configuration, 
            IAsyncQueryPandamart<PandamartPriceInventoryUpdate, Warehouse> pandamartPriceInventoryUpdateQuery, 
            IAsyncQuery<IReadOnlyDictionary<Warehouse, PandamartStore>> snrWarehousePandamartMappingsQuery, 
            IPandamartUpdateFileWriter writer, 
            IPandamartUpdatePathConventionFactory pandamartUpdatePathConventionFactory, 
            IDispatchService dispatchService,
            ISFTPPandamart sftpPandamart,
            ILocalDataPath localDataPath)
        {
            _configuration = configuration;
            _pandamartPriceInventoryUpdateQuery = pandamartPriceInventoryUpdateQuery;
            _snrWarehousePandamartMappingsQuery = snrWarehousePandamartMappingsQuery;
            _writer = writer;
            _pandamartUpdatePathConventionFactory = pandamartUpdatePathConventionFactory;
            _dispatchService = dispatchService;
            _sftpPandamart = sftpPandamart;
            _localDataPath = localDataPath;
        }

        //
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            //GET Warehouses number equivalent to Pandamart.
            //Get Warehouses code
            IReadOnlyDictionary<Warehouse, PandamartStore> warehouseMappings = await _snrWarehousePandamartMappingsQuery.ExecuteAsync(cancellationToken: cancellationToken);

            if (!warehouseMappings.Any()) throw new InvalidOperationException(message: "Unable to create updates for pandamart: No warehouse-pandamart mappings has been configured.");

            IPandamartUpdatePathConvention pandamartUpdatePathConvention = _pandamartUpdatePathConventionFactory.Create();

            //delete all existing file in SFTP
            await _sftpPandamart.MoveToNewFileLocation();

            //get the parent directory 
            var pandaMartLocalPath = _configuration.GetSection("Application:PandamartPriceInventoryUpdates:PandamartUpdatePathConventionFactory:StaticPrefixValuePathConvention:Prefix").Value;
            var LocalPath = string.Format("{0}\\{1}", Directory.GetParent(pandaMartLocalPath).FullName.ToString(), pandaMartLocalPath);
            await _localDataPath.CreateBackUpFile(LocalPath); //delete existing file

            await Parallel.ForEachAsync(
                source: warehouseMappings,
                parallelOptions: new ParallelOptions()
                {
                    CancellationToken = cancellationToken,
                    MaxDegreeOfParallelism = _configuration.GetValue(
                        key: "Application:ParallelOperationPreference",
                        defaultValue: 1)
                },
                body: async (KeyValuePair<Warehouse, PandamartStore> KeyValuePair, CancellationToken ct) =>
                {
                    Warehouse warehouse = KeyValuePair.Key;
                    PandamartStore pandamartStore = KeyValuePair.Value;

                    //Genarate Data to update
                    PandamartPriceInventoryUpdate update = await _pandamartPriceInventoryUpdateQuery.ExecuteAsyncPandamart(
                        parameter: warehouse,
                        cancellationToken: ct);

                    using (var memoryStream = new MemoryStream())
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
                            path: pandamartUpdatePathConvention.Localize(warehouse: warehouse, pandamartStore: pandamartStore), //Localize -> Genarate file path and file name
                            cancellationToken: cancellationToken);

                        //Send to SFTP
                        await _sftpPandamart.SendtoPAndamartsftp(pandamartUpdatePathConvention.Localize(warehouse: warehouse, pandamartStore: pandamartStore));

                    }
                });

        }
    }
}
