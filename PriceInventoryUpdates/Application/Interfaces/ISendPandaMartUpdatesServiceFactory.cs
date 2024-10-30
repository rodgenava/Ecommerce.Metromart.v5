using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.Enum;

namespace Application
{
    public interface ISendPandaMartUpdatesServiceFactory
    {
        ISendPandaMartUpdatesService CreateSendPandaMartUpdatesService(UpdateTypepanda updateType);
    }
}
