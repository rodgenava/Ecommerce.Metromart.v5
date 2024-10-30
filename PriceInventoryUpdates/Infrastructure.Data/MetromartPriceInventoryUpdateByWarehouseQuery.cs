using System.Data.Common;
using System.Data;
using Data.Common.Contracts;
using Data.Projections;
using Data.Sql;
using Data.Sql.Provider;
using Data.Sql.Mapping;
using static System.Formats.Asn1.AsnWriter;
using Application;

namespace Infrastructure.Data
{
    public class MetromartPriceInventoryUpdateByWarehouseQuery : IAsyncQuery<MetromartPriceInventoryUpdate, Warehouse>
    {
        private readonly ISqlProvider _provider;
        private readonly ISqlCaller _caller;
        private readonly int _commandTimeout;

        public MetromartPriceInventoryUpdateByWarehouseQuery(string connectionString, int commandTimeout)
        {
            _caller = new SqlCaller(_provider = new SqlServerProvider(connectionString));
            _commandTimeout = commandTimeout;
        }

        public async Task<MetromartPriceInventoryUpdate> ExecuteAsync(Warehouse parameter, CancellationToken cancellationToken)
        {
            // push-full-updates

            using DbCommand command = _provider.CreateCommand("Create Table #MetromartSkus(Sku Int Primary Key)Insert Into #MetromartSkus(Sku)Select Sku From Pricebook2Items Where InMetroMart=1 And Store=@Store Create Table #MetromartItems(Store Int,Sku Int,IsConsignment Bit,OnHand Decimal(15,3),Threshold Int,OnlinePrice Decimal(18,2),RegularPrice Decimal(18,2),CurrentPrice Decimal(18,2),InException Bit,IsDeliveryCharge Bit,Primary Key(Store,Sku))Insert Into #MetromartItems(Store,Sku,IsConsignment,OnHand,Threshold,OnlinePrice,RegularPrice,CurrentPrice,InException,IsDeliveryCharge)Select M.Store,M.Sku,M.IsConsignment,M.OnHand,M.Threshold,M.OnlinePrice,M.RegularPrice,M.CurrentPrice,M.InException,Cast(Case When M.NeedsAdjustment=1 And M.OnlinePrice=M.AdjustedPrice Then 1 Else 0 End As Bit)From Pricebook2ComputationBase M Join MetromartProductConfigurations MPC On M.Store=MPC.Store And M.Sku=MPC.Sku Where M.Store=@Store Select M.Store,M.Sku,M.Threshold,M.OnHand,M.RegularPrice,M.CurrentPrice,M.OnlinePrice,Cast(Case When CA.Store Is Not Null Then 1 Else 0 End As Bit)As IsConsignmentStore,M.IsConsignment,M.InException,M.IsDeliveryCharge From #MetromartItems M Left Join ConsignmentActivations CA On M.Store=CA.Store;Drop Table #MetromartSkus;Drop Table #MetromartItems;");

            //code genarated --> command
            //Select M.Store,M.Sku,M.Threshold,M.OnHand,M.RegularPrice,M.CurrentPrice,M.OnlinePrice,
            //Cast(Case When CA.Store Is Not Null Then 1 Else 0 End As Bit)As IsConsignmentStore, M.IsConsignment,M.InException,M.IsDeliveryCharge
            //From(Select M.Store, M.Sku, M.IsConsignment, M.OnHand, M.Threshold, M.OnlinePrice, M.RegularPrice, M.CurrentPrice, M.InException,
            //Cast(Case When M.NeedsAdjustment = 1 And M.OnlinePrice = M.AdjustedPrice Then 1 Else 0 End As Bit) as IsDeliveryCharge
            //From[Ecommerce.Development].[dbo].Pricebook2ComputationBase M Join[Ecommerce.Development].[dbo].MetromartProductConfigurations MPC On M.Store = MPC.Store And M.Sku = MPC.Sku Where M.Store = @Store)M
            //Left Join[Ecommerce.Development].[dbo].ConsignmentActivations CA On M.Store = CA.Store;

            command.CommandTimeout = _commandTimeout;

            command.Parameters.Add(_provider.CreateInputParameter(
                parameterName: "@Store",
                value: parameter.Code,
                dbType: DbType.Int32));

            IEnumerable<MetromartPriceInventoryUpdateItemDataHolder> items = await _caller.GetAsync(
               dataMapper: new ReflectionDataMapper<MetromartPriceInventoryUpdateItemDataHolder>(),
               command: command,
               cancellationToken: cancellationToken);

            return new MetromartPriceInventoryUpdate(
                Warehouse: parameter,
                Items: from item in items
                       select new MetromartPriceInventoryUpdateItem(
                           Sku: item.Sku,
                           Threshold: item.Threshold,
                           OnHand: item.OnHand,
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