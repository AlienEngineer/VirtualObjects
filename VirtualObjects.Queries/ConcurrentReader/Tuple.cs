using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualObjects.Queries.ConcurrentReader
{
    /// <summary>
    /// Represents a collection of values
    /// </summary>
    public interface ITuple
    {
        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        IEnumerable<String> Columns { get; }

        IConcurrentDataReader Reader { get; }

        /// <summary>
        /// Gets the value of the given column.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        T GetValue<T>(String column);

        /// <summary>
        /// Gets the value of the given column index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        T GetValue<T>(int index);

        /// <summary>
        /// Gets the value of the given column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        Object GetValue(String column);

        /// <summary>
        /// Gets the value of the given column index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        Object GetValue(int index);

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <returns></returns>
        Object[] GetValues();

        /// <summary>
        /// Gets the name of a column given its index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        String GetName(int index);
    }

    public class Tuple : ITuple
    {
        private readonly IDictionary<String, Object> data;
        private IList<Object> dataList;
        private IList<String> keyList;

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
