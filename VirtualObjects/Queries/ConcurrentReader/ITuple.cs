using System;
using System.Collections.Generic;

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
        IEnumerable<string> Columns { get; }

        /// <summary>
        /// Gets the reader.
        /// </summary>
        /// <value>
        /// The reader.
        /// </value>
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
}