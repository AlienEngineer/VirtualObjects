using System.Collections.Generic;
using System.Data;

namespace VirtualObjects.Queries.ConcurrentReader
{
    /// <summary>
    /// 
    /// </summary>
    public interface IConcurrentDataReader : IDataReader
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns></returns>
        ITuple GetData();
        /// <summary>
        /// Gets the tuples.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ITuple> GetTuples();
    }
}
