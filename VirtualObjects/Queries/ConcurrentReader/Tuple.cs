using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualObjects.Queries.ConcurrentReader
{
    /// <summary>
    /// 
    /// </summary>
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

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public IEnumerable<String> Columns
        {
            get
            {
                return data.Keys;
            }
        }

        /// <summary>
        /// Gets the value of the given column.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public T GetValue<T>(String column)
        {
            return (T) Convert.ChangeType(GetValue(column), typeof (T));
        }

        /// <summary>
        /// Gets the value of the given column index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public T GetValue<T>(int index)
        {
            return (T) Convert.ChangeType(GetValue(index), typeof (T));
        }

        /// <summary>
        /// Gets the value of the given column index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public object GetValue(int index)
        {
            if ( dataList == null )
            {
                dataList = data.Values.ToList();
            }
            return dataList[index];
        }

        /// <summary>
        /// Gets the value of the given column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public object GetValue(string column)
        {
            return data[column.ToLower()];
        }


        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <returns></returns>
        public object[] GetValues()
        {
            if ( dataList == null )
            {
                dataList = data.Values.ToList();
            }
            return dataList.ToArray();
        }

        /// <summary>
        /// Gets the name of a column given its index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public string GetName(int index)
        {
            if ( keyList == null )
            {
                keyList = data.Keys.ToList();
            }
            return keyList[index];
        }

        /// <summary>
        /// Gets the reader.
        /// </summary>
        /// <value>
        /// The reader.
        /// </value>
        public IConcurrentDataReader Reader { get; private set; }
    }
}
