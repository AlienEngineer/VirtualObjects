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

    class DataRowTuple : ITuple
    {
        private readonly DataRow _row;


        public DataRowTuple(DataRow row)
        {
            _row = row;
            Columns = _row.ColumnNames;
        }

        public IEnumerable<string> Columns { get; private set; }
        
        public IConcurrentDataReader Reader { get; private set; }
        
        public T GetValue<T>(string column)
        {
            throw new NotImplementedException();
        }

        public T GetValue<T>(int index)
        {
            return (T)Convert.ChangeType(_row.Values[index], typeof(T));
        }

        public object GetValue(string column)
        {
            throw new NotImplementedException();
        }

        public object GetValue(int index)
        {
            return _row.Values[index];
        }

        public object[] GetValues()
        {
            return _row.Values;
        }

        public string GetName(int index)
        {
            return _row.ColumnNames[index];
        }
    }
}
