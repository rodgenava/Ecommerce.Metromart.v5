namespace Data.Projections
{
    public record MetromartPriceInventoryUpdateItem(
        int Sku,
        int Threshold,
        decimal OnHand,
        decimal RegularPrice,
        decimal CurrentPrice,
        decimal OnlinePrice,
        bool IsConsignmentStore,
        bool IsConsignmentItem,
        bool InException,
        bool IsDeliveryChargedItem);

    public record MetromartPriceInventoryUpdate(
        Warehouse Warehouse,
        IEnumerable<MetromartPriceInventoryUpdateItem> Items);
}