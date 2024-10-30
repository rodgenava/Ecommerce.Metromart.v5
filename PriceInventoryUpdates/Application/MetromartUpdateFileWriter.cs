using Data.Projections;
using System.Text;

namespace Application
{
    public class MetromartUpdateFileWriter : IMetromartUpdateFileWriter
    {
        public async Task WriteToStreamAsync(Stream updateStream, IEnumerable<MetromartPriceInventoryUpdateItem> items, CancellationToken cancellationToken = default)
        {
            using var writer = new StreamWriter(
               stream: updateStream,
               encoding: Encoding.UTF8,
               leaveOpen: true);

            await writer.WriteLineAsync("External SKU,Base Amount,Percent Off,Status,Future_Buy_X_Take_Y_Start_At,Future_Buy_X_Take_Y_End_At,Future_Buy_X,Future_Take_Y");

            foreach(MetromartPriceInventoryUpdateItem item in items)
            {
                await writer.WriteLineAsync(Translate(item));
            }

            await writer.FlushAsync();
        }

        private static string Translate(MetromartPriceInventoryUpdateItem item)
        {
            item.Deconstruct(
                out int sku,
                out int threshold,
                out decimal onHand,
                out decimal regularPrice,
                out decimal _,  //currentPrice
                out decimal onlinePrice,
                out bool isConsignmentStore,
                out bool _, //isConsignmentItem
                out bool _, //inException
                out bool _);    //isDeliveryChargedItem

            string status = isConsignmentStore ?
                (onHand > (threshold > -1 ? threshold : 0)) ? "Available" : "Discontinued" :
                (onHand >= threshold) || threshold < 0 ? "Available" : "Discontinued";

            decimal percentOff = regularPrice == 0 || regularPrice < onlinePrice ? 0 : (regularPrice - onlinePrice) / regularPrice * 100;

            decimal basePrice = onlinePrice < regularPrice ? regularPrice : onlinePrice;

            //decimal markUpValue = isDeliveryChargedItem ? onlinePrice : 0;

            //decimal regularPriceValue = onlinePrice > regularPrice ? onlinePrice : regularPrice;

            return $"{sku},{basePrice:0.##},{percentOff:0.##},{status},,,,";
        }
    }

    public class MetromartUpdateFileWriter20221123 : IMetromartUpdateFileWriter
    {
        public async Task WriteToStreamAsync(Stream updateStream, IEnumerable<MetromartPriceInventoryUpdateItem> items, CancellationToken cancellationToken = default)
        {
            using var writer = new StreamWriter(
               stream: updateStream,
               encoding: Encoding.UTF8,
               leaveOpen: true);

            await writer.WriteLineAsync("External SKU,Base Amount,Percent Off,Status,Future_Buy_X_Take_Y_Start_At,Future_Buy_X_Take_Y_End_At,Future_Buy_X,Future_Take_Y");

            foreach(MetromartPriceInventoryUpdateItem item in items)
            {
                await writer.WriteLineAsync(Translate(item));
            }

            await writer.FlushAsync();
        }

        private static string Translate(MetromartPriceInventoryUpdateItem item)
        {
            item.Deconstruct(
                out int sku,
                out int threshold,
                out decimal onHand,
                out decimal regularPrice,
                out decimal _,  //currentPrice
                out decimal onlinePrice,
                out bool isConsignmentStore,
                out bool _, //isConsignmentItem
                out bool _, //inException
                out bool _);    //isDeliveryChargedItem

            string status = isConsignmentStore ?
                (onHand > (threshold > -1 ? threshold : 0)) ? "Available" : "Out" :
                (onHand >= threshold) || threshold < 0 ? "Available" : "Out";

            decimal percentOff = regularPrice == 0 || regularPrice < onlinePrice ? 0 : (regularPrice - onlinePrice) / regularPrice * 100;

            decimal basePrice = onlinePrice < regularPrice ? regularPrice : onlinePrice;

            //decimal markUpValue = isDeliveryChargedItem ? onlinePrice : 0;

            //decimal regularPriceValue = onlinePrice > regularPrice ? onlinePrice : regularPrice;

            return $"{sku},{basePrice:0.##},{percentOff:0.##},{status},,,,";
        }
    }
    
    public class MetromartUpdateFileWriter20221124 : IMetromartUpdateFileWriter
    {
        public async Task WriteToStreamAsync(Stream updateStream, IEnumerable<MetromartPriceInventoryUpdateItem> items, CancellationToken cancellationToken = default)
        {
            using var writer = new StreamWriter(
               stream: updateStream,
               encoding: Encoding.UTF8,
               leaveOpen: true);

            await writer.WriteLineAsync("External SKU,Base Amount,Percent Off,Status,Future_Buy_X_Take_Y_Start_At,Future_Buy_X_Take_Y_End_At,Future_Buy_X,Future_Take_Y");

            foreach(MetromartPriceInventoryUpdateItem item in items)
            {
                await writer.WriteLineAsync(Translate(item));
            }

            await writer.FlushAsync();
        }

        private static string Translate(MetromartPriceInventoryUpdateItem item)
        {
            item.Deconstruct(
                out int sku,
                out int threshold,
                out decimal onHand,
                out decimal regularPrice,
                out decimal _,  //currentPrice
                out decimal onlinePrice,
                out bool isConsignmentStore,
                out bool _, //isConsignmentItem
                out bool _, //inException
                out bool _);    //isDeliveryChargedItem

            string status = isConsignmentStore ?
                (onHand > (threshold > -1 ? threshold : 0)) ? "Available" : "Discontinued" :
                (onHand >= threshold) || threshold < 0 ? "Available" : "Discontinued";

            decimal percentOff = regularPrice == 0 || regularPrice < onlinePrice ? 0 : (regularPrice - onlinePrice) / regularPrice * 100;

            decimal basePrice = onlinePrice < regularPrice ? regularPrice : onlinePrice;

            //decimal markUpValue = isDeliveryChargedItem ? onlinePrice : 0;

            //decimal regularPriceValue = onlinePrice > regularPrice ? onlinePrice : regularPrice;

            string percentOffText = percentOff > 0 ? percentOff.ToString("0.##") : string.Empty;

            return $"{sku},{basePrice:0.##},{percentOffText},{status},,,,";
        }
    }

    public class MetromartUpdateFileWriter20221125 : IMetromartUpdateFileWriter
    {
        public async Task WriteToStreamAsync(Stream updateStream, IEnumerable<MetromartPriceInventoryUpdateItem> items, CancellationToken cancellationToken = default)
        {
            using var writer = new StreamWriter(
               stream: updateStream,
               encoding: Encoding.UTF8,
               leaveOpen: true);

            await writer.WriteLineAsync("External SKU,Base Amount,Percent Off,Status,Future_Buy_X_Take_Y_Start_At,Future_Buy_X_Take_Y_End_At,Future_Buy_X,Future_Take_Y");

            foreach (MetromartPriceInventoryUpdateItem item in items)
            {
                await writer.WriteLineAsync(Translate(item));
            }

            await writer.FlushAsync();
        }

        private static string Translate(MetromartPriceInventoryUpdateItem item)
        {
            item.Deconstruct(
                out int sku,
                out int threshold,
                out decimal onHand,
                out decimal regularPrice,
                out decimal _,  //currentPrice
                out decimal onlinePrice,
                out bool isConsignmentStore,
                out bool _, //isConsignmentItem
                out bool _, //inException
                out bool _);    //isDeliveryChargedItem

            string status = isConsignmentStore ?
                (onHand > (threshold > -1 ? threshold : 0)) ? "available" : "discontinued" :
                (onHand >= threshold) || threshold < 0 ? "available" : "discontinued";

            decimal percentOff = regularPrice == 0 || regularPrice < onlinePrice ? 0 : (regularPrice - onlinePrice) / regularPrice * 100;

            decimal basePrice = onlinePrice < regularPrice ? regularPrice : onlinePrice;

            //decimal markUpValue = isDeliveryChargedItem ? onlinePrice : 0;

            //decimal regularPriceValue = onlinePrice > regularPrice ? onlinePrice : regularPrice;

            string percentOffText = percentOff > 0 ? percentOff.ToString("0.##") : string.Empty;

            return $"{sku},{basePrice:0.##},{percentOffText},{status},,,,";
        }
    }
    
    public class MetromartUpdateFileWriter20221220 : IMetromartUpdateFileWriter
    {
        public async Task WriteToStreamAsync(Stream updateStream, IEnumerable<MetromartPriceInventoryUpdateItem> items, CancellationToken cancellationToken = default)
        {
            using var writer = new StreamWriter(
               stream: updateStream,
               encoding: Encoding.UTF8,
               leaveOpen: true);

            await writer.WriteLineAsync("External SKU,Base Amount,Percent Off,Status,Future_Buy_X_Take_Y_Start_At,Future_Buy_X_Take_Y_End_At,Future_Buy_X,Future_Take_Y");

            foreach (MetromartPriceInventoryUpdateItem item in items)
            {
                await writer.WriteLineAsync(Translate(item));
            }

            await writer.FlushAsync();
        }

        private static string Translate(MetromartPriceInventoryUpdateItem item)
        {
            item.Deconstruct(
                out int sku,
                out int threshold,
                out decimal onHand,
                out decimal regularPrice,
                out decimal _,  //currentPrice
                out decimal onlinePrice,
                out bool isConsignmentStore,
                out bool _, //isConsignmentItem
                out bool _, //inException
                out bool _);    //isDeliveryChargedItem

            string status = isConsignmentStore ?
                (onHand > (threshold > -1 ? threshold : 0)) ? "available" : "discontinued" :
                (onHand >= threshold) || threshold < 0 ? "available" : "discontinued";

            decimal percentOff = regularPrice == 0 || regularPrice < onlinePrice ? 0 : (regularPrice - onlinePrice) / regularPrice * 100;

            decimal basePrice = onlinePrice < regularPrice ? regularPrice : onlinePrice;

            //decimal markUpValue = isDeliveryChargedItem ? onlinePrice : 0;

            //decimal regularPriceValue = onlinePrice > regularPrice ? onlinePrice : regularPrice;

            string percentOffText = percentOff > 0 ? percentOff.ToString() : string.Empty;

            return $"{sku},{basePrice:0.##},{percentOffText},{status},,,,";
        }
    }

    public class MetromartUpdateFileWriter20230113 : IMetromartUpdateFileWriter
    {
        public async Task WriteToStreamAsync(Stream updateStream, IEnumerable<MetromartPriceInventoryUpdateItem> items, CancellationToken cancellationToken = default)
        {
            using var writer = new StreamWriter(
               stream: updateStream,
               encoding: Encoding.UTF8,
               leaveOpen: true);

            await writer.WriteLineAsync("External SKU,Base Amount,Percent Off,Status,Future_Buy_X_Take_Y_Start_At,Future_Buy_X_Take_Y_End_At,Future_Buy_X,Future_Take_Y");

            foreach (MetromartPriceInventoryUpdateItem item in items)
            {
                await writer.WriteLineAsync(Translate(item));
            }

            await writer.FlushAsync();
        }

        private static string Translate(MetromartPriceInventoryUpdateItem item)
        {
            item.Deconstruct(
                out int sku,
                out int threshold,
                out decimal onHand,
                out decimal regularPrice,
                out decimal _,  //currentPrice
                out decimal onlinePrice,
                out bool isConsignmentStore,
                out bool isConsignmentItem,
                out bool _, //inException
                out bool _);    //isDeliveryChargedItem

            string status = isConsignmentStore ?
                (onHand > (threshold > -1 ? threshold : 0)) ? "available" : "discontinued" :
                !isConsignmentItem ? (onHand > (threshold > -1 ? threshold : 0)) ? "available" : "discontinued" : "available";

            decimal percentOff = regularPrice == 0 || regularPrice < onlinePrice ? 0 : (regularPrice - onlinePrice) / regularPrice * 100;

            decimal basePrice = onlinePrice < regularPrice ? regularPrice : onlinePrice;

            //decimal markUpValue = isDeliveryChargedItem ? onlinePrice : 0;

            //decimal regularPriceValue = onlinePrice > regularPrice ? onlinePrice : regularPrice;

            string percentOffText = percentOff > 0 ? percentOff.ToString() : string.Empty;

            return $"{sku},{basePrice:0.##},{percentOffText},{status},,,,";
        }
    }
}