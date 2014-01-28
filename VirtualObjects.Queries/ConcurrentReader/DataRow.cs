using System;
using System.Collections.Generic;

namespace VirtualObjects.Queries.ConcurrentReader
{
    /// <summary>
    /// Holds one row of data
    /// </summary>
    internal class DataRow
    {
        public DataRow(int index)
        {
            Index = index;
        }

        public int Index { get; private set; }

        public String[] ColumnNames { get; set; }

        public Object[] Values { get; set; }

        public ITuple ToTuple(IConcurrentDataReader reader)
        {
            var dic = new Dictionary<String, Object>();

            for ( var i = 0; i < ColumnNames.Length; i++ )
            {
                if ( dic.ContainsKey(ColumnNames[i]) )
                {
                    ColumnNames[i] += "_";
                }

                dic.Add(ColumnNames[i], Values[i]);
            }

            return new Tuple(dic, reader);
        }
    }
}
