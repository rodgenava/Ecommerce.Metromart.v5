using System.Collections.Generic;

namespace Data.Common.Contracts
{
    public class DataEvent
    {

    }

    public interface IDataEventSource
    {
        IEnumerable<DataEvent> ReleaseEvents();
    }
}
