namespace Infrastructure.Data
{
    internal class PandamartPriceInventoryUpdateItemDataHolder
    {
        public int Store { get; set; }
        public int Sku { get; set; }
        public int Threshold { get; set; }
        public decimal OnHand { get; set; }
        public string Status { get; set; }
        public decimal RegularPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal OnlinePrice { get; set; }
        public bool IsConsignmentStore { get; set; }
        public bool IsConsignment { get; set; }
        public bool InException { get; set; }
        public bool IsDeliveryCharge { get; set; }
    }
}
