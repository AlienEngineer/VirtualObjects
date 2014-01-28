using System.Collections.Generic;
using System.Data;

namespace VirtualObjects.Queries.ConcurrentReader
{
    public interface IConcurrentDataReader : IDataReader
    {
        ITuple GetData();
        IEnumerable<ITuple> GetTuples();
    }
}
