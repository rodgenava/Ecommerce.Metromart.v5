using Data.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class PandamartUpdateFileWriter : IPandamartUpdateFileWriter
    {
        public async Task WriteToStreamAsync(Stream updateStream, IEnumerable<PandamartPriceInventoryUpdateItem> items, CancellationToken cancellationToken = default)
        {
            using var writer = new StreamWriter(
               stream: updateStream,
               encoding: Encoding.UTF8,
               leaveOpen: true);

            await writer.WriteLineAsync("SKU,Barcode,Price,Status,Quantity,");

            foreach (PandamartPriceInventoryUpdateItem item in items)
            {
                await writer.WriteLineAsync(Translate(item));
            }

            await writer.FlushAsync();
        }

        private static string Translate(PandamartPriceInventoryUpdateItem item)
        {
            item.Deconstruct(
                out int sku,
                out int threshold,
                out decimal onHand,
                out string Status,
                out decimal regularPrice,
                out decimal _currentPrice,
                out decimal onlinePrice,
                out bool isConsignmentStore,
                out bool _, //isConsignmentItem
                out bool _, //inException
                out bool _);    //isDeliveryChargedItem

            //string status = isConsignmentStore ?
            //    (onHand > (threshold > -1 ? threshold : 0)) ? "Available" : "Discontinued" :
            //    (onHand >= threshold) || threshold < 0 ? "Available" : "Discontinued";

            //decimal percentOff = regularPrice == 0 || regularPrice < onlinePrice ? 0 : (regularPrice - onlinePrice) / regularPrice * 100;

            //decimal basePrice = onlinePrice < regularPrice ? regularPrice : onlinePrice;

            //decimal markUpValue = isDeliveryChargedItem ? onlinePrice : 0;

            //decimal regularPriceValue = onlinePrice > regularPrice ? onlinePrice : regularPrice;

            return $"{sku},,{_currentPrice},{Status},{onHand}";
        }
    }
}
