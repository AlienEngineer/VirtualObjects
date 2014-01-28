using System;
using System.Collections.Generic;
using System.Data;

namespace VirtualObjects.Queries.ConcurrentReader
{
    public abstract class ConcurrentDataReaderBase : IConcurrentDataReader
    {
        #region IConcurrentDataReader Members

        public int RecordsAffected { get; protected set; }

        public int FieldCount { get; protected set; }

        public int Depth { get; protected set; }

        public abstract void Dispose();

        public abstract void Close();

        public abstract ITuple GetData();

        public abstract bool Read();

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public bool IsClosed
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool NextResult()
        {
            return Read();
        }

        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

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

        public object this[int i]
        {
            get
            {
                return GetData().GetValue(i);
            }
        }


        public abstract IEnumerable<ITuple> GetTuples();

        #endregion

        #region GETTERS

        public bool GetBoolean(int i)
        {
            return GetData().GetValue<bool>(i);
        }

        public byte GetByte(int i)
        {
            return GetData().GetValue<byte>(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            return GetData().GetValue<char>(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            return GetData().GetValue<string>(i);
        }

        public DateTime GetDateTime(int i)
        {
            return GetData().GetValue<DateTime>(i);
        }

        public decimal GetDecimal(int i)
        {
            return GetData().GetValue<decimal>(i);
        }

        public double GetDouble(int i)
        {
            return GetData().GetValue<double>(i);
        }

        public Type GetFieldType(int i)
        {
            return GetData().GetValue(i).GetType();
        }

        public float GetFloat(int i)
        {
            return GetData().GetValue<float>(i);
        }

        public Guid GetGuid(int i)
        {
            return GetData().GetValue<Guid>(i);
        }

        public short GetInt16(int i)
        {
            return GetData().GetValue<Int16>(i);
        }

        public int GetInt32(int i)
        {
            return GetData().GetValue<int>(i);
        }

        public long GetInt64(int i)
        {
            return GetData().GetValue<long>(i);
        }

        public string GetName(int i)
        {
            return GetData().GetName(i);
        }

        public int GetOrdinal(string name)
        {
            return GetData().GetValue<int>(name);
        }

        public string GetString(int i)
        {
            return GetData().GetValue<String>(i);
        }

        public object GetValue(int i)
        {
            return this[i];
        }

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
