using System;
using System.Data;
using System.Linq;
using VirtualObjects.Exceptions;
using VirtualObjects.Queries;
using VirtualObjects.Queries.Mapping;

namespace VirtualObjects.CRUD.Operations
{
    internal class QueryOperation : IOperation
    {
        private readonly IEntityInfo _entityInfo;
        private QueryCommand _queryCommand;
        private readonly IEntitiesMapper _mapper;
        private readonly MapperContext _context;

        public QueryOperation(IEntityInfo entityInfo) : this(entityInfo, new EntityModelEntitiesMapper())
        {
            
        }

        public QueryOperation(IEntityInfo entityInfo, IEntitiesMapper mapper)
        {
            _entityInfo = entityInfo;
            _mapper = mapper;

            _context = new MapperContext
            {
                EntityInfo = entityInfo,
                OutputType = entityInfo.EntityType
            };
        }

        public string CommandText { get; set; }

        public object Execute(SessionContext sessionContext)
        {
            var connection = sessionContext.Connection;
            var commandText = _queryCommand.Command;
            var parameters = _queryCommand.Parameters.ToDictionary(
                e => e.Name,
                e => (IOperationParameter)new OperationParameter
                {
                    Name = e.Name,
                    Value = e.Value
                });

            var reader = new CustomReader(connection.ExecuteReader(commandText, parameters));

            return _mapper.MapEntities(reader, new QueryInfo
            {
                MapEntity = _entityInfo.MapEntity,
                EntityCast = _entityInfo.EntityCast,
                MakeEntity = _entityInfo.EntityProxyFactory
            }, _context.OutputType, sessionContext);
        }

        public IOperation PrepareOperation(object entityModel)
        {
            var queryCommand = entityModel as QueryCommand;

            if (queryCommand == null)
            {
                throw new ExecutionException(Errors.Operations_Invalid_Argument);
            }

            _queryCommand = queryCommand;

            return this;
        }
    }

    internal class CustomReader : IDataReader
    {
        private readonly IDataReader _executeReader;

        public CustomReader(IDataReader executeReader)
        {
            _executeReader = executeReader;
        }

        public void Dispose()
        {
            _executeReader.Dispose();
        }

        public string GetName(int i)
        {
            return _executeReader.GetName(i);
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public object GetValue(int i)
        {
            throw new NotImplementedException();
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

        public int FieldCount { get; }

        object IDataRecord.this[int i]
        {
            get { throw new NotImplementedException(); }
        }

        object IDataRecord.this[string name]
        {
            get { throw new NotImplementedException(); }
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        public bool Read()
        {
            throw new NotImplementedException();
        }

        public int Depth { get; }
        public bool IsClosed { get; }
        public int RecordsAffected { get; }
    }
}