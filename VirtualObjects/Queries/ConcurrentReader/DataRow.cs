using System;
using System.Collections.Generic;

namespace VirtualObjects.Queries.ConcurrentReader
{
    /// <summary>
    /// Holds one row of data
    /// </summary>
    internal class DataRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataRow"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        public DataRow(int index)
        {
            Index = index;
        }

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <value>
        /// The index.
        /// </value>
        public int Index { get; private set; }

        /// <summary>
        /// Gets or sets the column names.
        /// </summary>
        /// <value>
        /// The column names.
        /// </value>
        public String[] ColumnNames { get; set; }

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public Object[] Values { get; set; }

        /// <summary>
        /// To the tuple.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
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


        /// <summary>
        /// Initializes a new instance of the <see cref="DataRowTuple"/> class.
        /// </summary>
        /// <param name="row">The row.</param>
        public DataRowTuple(DataRow row)
        {
            _row = row;
            Columns = _row.ColumnNames;
        }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public IEnumerable<string> Columns { get; private set; }

        /// <summary>
        /// Gets the reader.
        /// </summary>
        /// <value>
        /// The reader.
        /// </value>
        public IConcurrentDataReader Reader { get; private set; }

        /// <summary>
        /// Gets the value of the given column.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public T GetValue<T>(string column)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the value of the given column index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public T GetValue<T>(int index)
        {
            return (T)Convert.ChangeType(_row.Values[index], typeof(T));
        }

        /// <summary>
        /// Gets the value of the given column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public object GetValue(string column)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the value of the given column index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public object GetValue(int index)
        {
            return _row.Values[index];
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <returns></returns>
        public object[] GetValues()
        {
            return _row.Values;
        }

        /// <summary>
        /// Gets the name of a column given its index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public string GetName(int index)
        {
            return _row.ColumnNames[index];
        }
    }
}
