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
    public class PandamartPriceInventoryUpdateByWarehouseQuery : IAsyncQueryPandamart<PandamartPriceInventoryUpdate, Warehouse>
    {
        private readonly ISqlProvider _provider;
        private readonly ISqlCaller _caller;
        private readonly int _commandTimeout;

        public PandamartPriceInventoryUpdateByWarehouseQuery(string connectionString, int commandTimeout)
        {
            _caller = new SqlCaller(_provider = new SqlServerProvider(connectionString));
            _commandTimeout = commandTimeout;
        }

        public async Task<PandamartPriceInventoryUpdate> ExecuteAsyncPandamart(Warehouse parameter, CancellationToken cancellationToken)
        {
            // push-full-updates

            using DbCommand command = _provider.CreateCommand("Create Table #PandamartSkus(Sku Int Primary Key)Insert Into #PandamartSkus(Sku)Select Sku From Pricebook2Items Where InPandaMart=1 And Store=@Store Create Table #PandamartItems(Store Int,Sku Int,IsConsignment Bit,OnHand Decimal(15,3),Threshold Int,OnlinePrice Decimal(18,2),RegularPrice Decimal(18,2),CurrentPrice Decimal(18,2),InException Bit,IsDeliveryCharge Bit,Primary Key(Store,Sku))Insert Into #PandamartItems(Store,Sku,IsConsignment,OnHand,Threshold,OnlinePrice,RegularPrice,CurrentPrice,InException,IsDeliveryCharge)Select M.Store,M.Sku,M.IsConsignment,M.OnHand,M.Threshold,M.OnlinePrice,M.RegularPrice,M.CurrentPrice,M.InException,Cast(Case When M.NeedsAdjustment=1 And M.OnlinePrice=M.AdjustedPrice Then 1 Else 0 End As Bit)From Pricebook2ComputationBase M Join PandamartProductConfigurations MPC On M.Store=MPC.Store And M.Sku=MPC.Sku Where M.Store=@Store Select M.Store,M.Sku,M.Threshold,M.OnHand,Cast(Case When M.OnHand > 0 Then 'Active' Else 'InActive' End as varchar) as [Status],M.RegularPrice,M.CurrentPrice,M.OnlinePrice,Cast(Case When CA.Store Is Not Null Then 1 Else 0 End As Bit)As IsConsignmentStore,M.IsConsignment,M.InException,M.IsDeliveryCharge From #PandamartItems M Left Join ConsignmentActivations CA On M.Store=CA.Store;Drop Table #PandamartSkus;Drop Table #PandamartItems;");

            //code genarated --> command
            //Select M.Store,M.Sku,M.Threshold,M.OnHand,Cast(Case When M.OnHand > 0 Then 'Active' Else 'InActive' End as varchar) as [Status],M.RegularPrice,M.CurrentPrice,M.OnlinePrice,
            //Cast(Case When CA.Store Is Not Null Then 1 Else 0 End As Bit)As IsConsignmentStore, M.IsConsignment,M.InException,M.IsDeliveryCharge
            //From(Select M.Store, M.Sku, M.IsConsignment, M.OnHand, M.Threshold, M.OnlinePrice, M.RegularPrice, M.CurrentPrice, M.InException,
            //Cast(Case When M.NeedsAdjustment = 1 And M.OnlinePrice = M.AdjustedPrice Then 1 Else 0 End As Bit) as IsDeliveryCharge
            //From[Ecommerce.Development].[dbo].Pricebook2ComputationBase M Join[Ecommerce.Development].[dbo].PandamartProductConfigurations MPC On M.Store = MPC.Store And M.Sku = MPC.Sku Where M.Store = @Store)M
            //Left Join[Ecommerce.Development].[dbo].ConsignmentActivations CA On M.Store = CA.Store;

            command.CommandTimeout = _commandTimeout;

            command.Parameters.Add(_provider.CreateInputParameter(
                parameterName: "@Store",
                value: parameter.Code,
                dbType: DbType.Int32));

            IEnumerable<PandamartPriceInventoryUpdateItemDataHolder> items = await _caller.GetAsync(
               dataMapper: new ReflectionDataMapper<PandamartPriceInventoryUpdateItemDataHolder>(),
               command: command,
               cancellationToken: cancellationToken);

            return new PandamartPriceInventoryUpdate(
                Warehouse: parameter,
                Items: from item in items
                       select new PandamartPriceInventoryUpdateItem(
                           Sku: item.Sku,
                           Threshold: item.Threshold,
                           OnHand: item.OnHand,
                           Status: item.Status,
                           RegularPrice: item.RegularPrice,
                           CurrentPrice: item.CurrentPrice,
                           OnlinePrice: item.OnlinePrice,
                           IsConsignmentStore: item.IsConsignmentStore,
                           IsConsignmentItem: item.IsConsignment,
                           InException: item.InException,
                           IsDeliveryChargedItem: item.IsDeliveryCharge));
        }
    }
}
