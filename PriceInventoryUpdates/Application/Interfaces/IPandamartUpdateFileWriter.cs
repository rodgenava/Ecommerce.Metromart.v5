using Data.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public interface IPandamartUpdateFileWriter
    {
        Task WriteToStreamAsync(Stream updateStream, IEnumerable<PandamartPriceInventoryUpdateItem> items, CancellationToken cancellationToken = default);
    }
}
