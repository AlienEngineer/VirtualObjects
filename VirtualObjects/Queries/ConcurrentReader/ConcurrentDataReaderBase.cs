using System;
using System.Collections.Generic;
using System.Data;

namespace VirtualObjects.Queries.ConcurrentReader
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ConcurrentDataReaderBase : IConcurrentDataReader
    {
        #region IConcurrentDataReader Members

        /// <summary>
        /// Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.
        /// </summary>
        /// <returns>The number of rows changed, inserted, or deleted; 0 if no rows were affected or the statement failed; and -1 for SELECT statements.</returns>
        public int RecordsAffected { get; protected set; }

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        /// <returns>When not positioned in a valid recordset, 0; otherwise, the number of columns in the current record. The default is -1.</returns>
        public int FieldCount { get; protected set; }

        /// <summary>
        /// Gets a value indicating the depth of nesting for the current row.
        /// </summary>
        /// <returns>The level of nesting.</returns>
        public int Depth { get; protected set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Closes the <see cref="T:System.Data.IDataReader" /> Object.
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns></returns>
        public abstract ITuple GetData();

        /// <summary>
        /// Advances the <see cref="T:System.Data.IDataReader" /> to the next record.
        /// </summary>
        /// <returns>
        /// true if there are more rows; otherwise, false.
        /// </returns>
        public abstract bool Read();

        /// <summary>
        /// Returns a <see cref="T:System.Data.DataTable" /> that describes the column metadata of the <see cref="T:System.Data.IDataReader" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Data.DataTable" /> that describes the column metadata.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a value indicating whether the data reader is closed.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        /// <returns>true if the data reader is closed; otherwise, false.</returns>
        public bool IsClosed
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Advances the data reader to the next result, when reading the results of batch SQL statements.
        /// </summary>
        /// <returns>
        /// true if there are more rows; otherwise, false.
        /// </returns>
        public bool NextResult()
        {
            return Read();
        }

        /// <summary>
        /// Return whether the specified field is set to null.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// true if the specified field is set to null; otherwise, false.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the column with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">The field  + name +  was not found.</exception>
        public object this[string name]
        {
            get
            {
                try
                {
                    return GetData().GetValue(name);
                }
                catch (KeyNotFoundException ex)
                {
                    throw new KeyNotFoundException("The field " + name + " was not found.", ex);
                }
            }
        }

        /// <summary>
        /// Gets the column located at the specified index.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        public object this[int i]
        {
            get
            {
                return GetData().GetValue(i);
            }
        }


        /// <summary>
        /// Gets the tuples.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<ITuple> GetTuples();

        #endregion

        #region GETTERS

        /// <summary>
        /// Gets the value of the specified column as a Boolean.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>
        /// The value of the column.
        /// </returns>
        public bool GetBoolean(int i)
        {
            return GetData().GetValue<bool>(i);
        }

        /// <summary>
        /// Gets the 8-bit unsigned integer value of the specified column.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>
        /// The 8-bit unsigned integer value of the specified column.
        /// </returns>
        public byte GetByte(int i)
        {
            return GetData().GetValue<byte>(i);
        }

        /// <summary>
        /// Reads a stream of bytes from the specified column offset into the buffer as an array, starting at the given buffer offset.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="fieldOffset">The index within the field from which to start the read operation.</param>
        /// <param name="buffer">The buffer into which to read the stream of bytes.</param>
        /// <param name="bufferoffset">The index for <paramref name="buffer" /> to start the read operation.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>
        /// The actual number of bytes read.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the character value of the specified column.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>
        /// The character value of the specified column.
        /// </returns>
        public char GetChar(int i)
        {
            return GetData().GetValue<char>(i);
        }

        /// <summary>
        /// Reads a stream of characters from the specified column offset into the buffer as an array, starting at the given buffer offset.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <param name="fieldoffset">The index within the row from which to start the read operation.</param>
        /// <param name="buffer">The buffer into which to read the stream of bytes.</param>
        /// <param name="bufferoffset">The index for <paramref name="buffer" /> to start the read operation.</param>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>
        /// The actual number of characters read.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an <see cref="T:System.Data.IDataReader" /> for the specified column ordinal.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The <see cref="T:System.Data.IDataReader" /> for the specified column ordinal.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the data type information for the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The data type information for the specified field.
        /// </returns>
        public string GetDataTypeName(int i)
        {
            return GetData().GetValue<string>(i);
        }

        /// <summary>
        /// Gets the date and time data value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The date and time data value of the specified field.
        /// </returns>
        public DateTime GetDateTime(int i)
        {
            return GetData().GetValue<DateTime>(i);
        }

        /// <summary>
        /// Gets the fixed-position numeric value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The fixed-position numeric value of the specified field.
        /// </returns>
        public decimal GetDecimal(int i)
        {
            return GetData().GetValue<decimal>(i);
        }

        /// <summary>
        /// Gets the double-precision floating point number of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The double-precision floating point number of the specified field.
        /// </returns>
        public double GetDouble(int i)
        {
            return GetData().GetValue<double>(i);
        }

        /// <summary>
        /// Gets the <see cref="T:System.Type" /> information corresponding to the type of <see cref="T:System.Object" /> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)" />.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The <see cref="T:System.Type" /> information corresponding to the type of <see cref="T:System.Object" /> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)" />.
        /// </returns>
        public Type GetFieldType(int i)
        {
            return GetData().GetValue(i).GetType();
        }

        /// <summary>
        /// Gets the single-precision floating point number of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The single-precision floating point number of the specified field.
        /// </returns>
        public float GetFloat(int i)
        {
            return GetData().GetValue<float>(i);
        }

        /// <summary>
        /// Returns the GUID value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The GUID value of the specified field.
        /// </returns>
        public Guid GetGuid(int i)
        {
            return GetData().GetValue<Guid>(i);
        }

        /// <summary>
        /// Gets the 16-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The 16-bit signed integer value of the specified field.
        /// </returns>
        public short GetInt16(int i)
        {
            return GetData().GetValue<Int16>(i);
        }

        /// <summary>
        /// Gets the 32-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The 32-bit signed integer value of the specified field.
        /// </returns>
        public int GetInt32(int i)
        {
            return GetData().GetValue<int>(i);
        }

        /// <summary>
        /// Gets the 64-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The 64-bit signed integer value of the specified field.
        /// </returns>
        public long GetInt64(int i)
        {
            return GetData().GetValue<long>(i);
        }

        /// <summary>
        /// Gets the name for the field to find.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The name of the field or the empty string (""), if there is no value to return.
        /// </returns>
        public string GetName(int i)
        {
            return GetData().GetName(i);
        }

        /// <summary>
        /// Return the index of the named field.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        /// <returns>
        /// The index of the named field.
        /// </returns>
        public int GetOrdinal(string name)
        {
            return GetData().GetValue<int>(name);
        }

        /// <summary>
        /// Gets the string value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The string value of the specified field.
        /// </returns>
        public string GetString(int i)
        {
            return GetData().GetValue<String>(i);
        }

        /// <summary>
        /// Return the value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>
        /// The <see cref="T:System.Object" /> which will contain the field value upon return.
        /// </returns>
        public object GetValue(int i)
        {
            return this[i];
        }

        /// <summary>
        /// Populates an array of objects with the column values of the current record.
        /// </summary>
        /// <param name="values">An array of <see cref="T:System.Object" /> to copy the attribute fields into.</param>
        /// <returns>
        /// The number of instances of <see cref="T:System.Object" /> in the array.
        /// </returns>
        public int GetValues(object[] values)
        {
            ITuple tuple = GetData();
            object[] objects = tuple.GetValues();
            for ( int i = 0; i < values.Length; i++ )
            {
                values[i] = objects[i];
            }
            return objects.Length;
        }

        #endregion
    }
}
