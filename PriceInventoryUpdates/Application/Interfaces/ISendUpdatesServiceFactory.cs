using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.Enum;

namespace Application
{
    public interface ISendUpdatesServiceFactory
    {
        ISendUpdatesService CreateSendUpdatesService(UpdateType updateType);
    }
}
