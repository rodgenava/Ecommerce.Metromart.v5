using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public interface IDispatchService
    {
        Task SendAsync(Stream item, string path, CancellationToken cancellationToken);
    }
}
