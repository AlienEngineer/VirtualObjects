using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualObjects.Queries.ConcurrentReader
{
    public class Tuple : ITuple
    {
        private readonly IDictionary<String, Object> data;
        private IList<Object> dataList;
        private IList<String> keyList;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tuple"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="reader">The reader.</param>
        public Tuple(IDictionary<String, Object> data, IConcurrentDataReader reader)
        {
            this.data = data;
            Reader = reader;
        }

        public IEnumerable<String> Columns
        {
            get
            {
                return data.Keys;
            }
        }

        public T GetValue<T>(String column)
        {
            return (T) Convert.ChangeType(GetValue(column), typeof (T));
        }

        public T GetValue<T>(int index)
        {
            return (T) Convert.ChangeType(GetValue(index), typeof (T));
        }

        public object GetValue(int index)
        {
            if ( dataList == null )
            {
                dataList = data.Values.ToList();
            }
            return dataList[index];
        }

        public object GetValue(string column)
        {
            return data[column.ToLower()];
        }


        public object[] GetValues()
        {
            if ( dataList == null )
            {
                dataList = data.Values.ToList();
            }
            return dataList.ToArray();
        }

        public string GetName(int index)
        {
            if ( keyList == null )
            {
                keyList = data.Keys.ToList();
            }
            return keyList[index];
        }

        public IConcurrentDataReader Reader { get; private set; }
    }
}
