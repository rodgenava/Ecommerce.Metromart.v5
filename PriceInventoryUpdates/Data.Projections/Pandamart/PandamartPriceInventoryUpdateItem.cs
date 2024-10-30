namespace Data.Projections
{
    public record PandamartPriceInventoryUpdateItem(
    int Sku,
    int Threshold,
    decimal OnHand,
    string Status,
    decimal RegularPrice,
    decimal CurrentPrice,
    decimal OnlinePrice,
    bool IsConsignmentStore,
    bool IsConsignmentItem,
    bool InException,
    bool IsDeliveryChargedItem);

    public record PandamartPriceInventoryUpdate(
        Warehouse Warehouse,
        IEnumerable<PandamartPriceInventoryUpdateItem> Items);
}
