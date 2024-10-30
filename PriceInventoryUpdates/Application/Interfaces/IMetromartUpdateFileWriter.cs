using Data.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public interface IMetromartUpdateFileWriter
    {
        Task WriteToStreamAsync(Stream updateStream, IEnumerable<MetromartPriceInventoryUpdateItem> items, CancellationToken cancellationToken = default);
    }
}
