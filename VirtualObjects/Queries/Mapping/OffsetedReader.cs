using System;
using System.Data;

namespace VirtualObjects.Queries.Mapping
{
    class OffsetedReader : IDataReader
    {
        private readonly int _offset;
        private readonly IDataReader _reader;
        private DataTable _dataTable;

        public OffsetedReader(IDataReader reader, int offset)
        {
            _offset = offset;
            _reader = reader;
        }
        
        #region IDataReader Members

        public void Close()
        {
            _reader.Close();
        }

        public int Depth => _reader.Depth;

        public DataTable GetSchemaTable()
        {
            return _dataTable ?? (_dataTable = _reader.GetSchemaTable());
        }

        public bool IsClosed => _reader.IsClosed;

        public bool NextResult()
        {
            return _reader.NextResult();
        }

        public bool Read()
        {
            return _reader.Read();
        }

        public int RecordsAffected => _reader.RecordsAffected;

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _reader.Dispose();
        }

        #endregion

        #region IDataRecord Members

        public virtual int FieldCount => _reader.FieldCount;

        public bool GetBoolean(int i)
        {
            return _reader.GetBoolean(i + _offset);
        }

        public byte GetByte(int i)
        {
            return _reader.GetByte(i + _offset);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return _reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            return _reader.GetChar(i + _offset);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return _reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public IDataReader GetData(int i)
        {
            return _reader.GetData(i + _offset);
        }

        public string GetDataTypeName(int i)
        {
            return _reader.GetDataTypeName(i + _offset);
        }

        public DateTime GetDateTime(int i)
        {
            return _reader.GetDateTime(i + _offset);
        }

        public decimal GetDecimal(int i)
        {
            return _reader.GetDecimal(i + _offset);
        }

        public double GetDouble(int i)
        {
            return _reader.GetDouble(i + _offset);
        }

        public Type GetFieldType(int i)
        {
            return _reader.GetFieldType(i + _offset);
        }

        public float GetFloat(int i)
        {
            return _reader.GetFloat(i + _offset);
        }

        public Guid GetGuid(int i)
        {
            return _reader.GetGuid(i + _offset);
        }

        public short GetInt16(int i)
        {
            return _reader.GetInt16(i + _offset);
        }

        public int GetInt32(int i)
        {
            return _reader.GetInt32(i + _offset);
        }

        public long GetInt64(int i)
        {
            return _reader.GetInt64(i + _offset);
        }

        public string GetName(int i)
        {
            return _reader.GetName(i + _offset);
        }

        public int GetOrdinal(string name)
        {
            return _reader.GetOrdinal(name);
        }

        public string GetString(int i)
        {
            return _reader.GetString(i + _offset);
        }

        public object GetValue(int i)
        {
            return _reader.GetValue(i + _offset);
        }

        public virtual int GetValues(object[] values)
        {

            var length = FieldCount - _offset;
            _reader.GetValues(values);

            if (_offset <= 0) return length;

            for ( var i = 0; i < length; i++ )
            {
                values[i] = values[_offset + i];
            }

            return length;
        }

        public bool IsDBNull(int i)
        {
            return _reader.IsDBNull(i + _offset);
        }

        public object this[string name] => _reader[name];

        public object this[int i] => _reader[i + _offset];

        #endregion
    }
}