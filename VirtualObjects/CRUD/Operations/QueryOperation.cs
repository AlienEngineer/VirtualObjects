using System;
using System.Collections.Generic;
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

            var reader = new CustomReader(_entityInfo, connection.ExecuteReader(commandText, parameters));

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

    internal class CustomReader : OffsetedReader
    {
        private readonly IEntityInfo _entityInfo;
        private readonly IDataReader _executeReader;
        private IEnumerable<SwapIndexes> _swapIndexeses;

        class SwapIndexes
        {
            public int Index1 { get; set; }
            public int Index2 { get; set; }
        }

        public CustomReader(IEntityInfo entityInfo, IDataReader executeReader) : base(executeReader, 0)
        {
            _entityInfo = entityInfo;
            _executeReader = executeReader;
            _swapIndexeses = null;
        }

        public override int GetValues(object[] values)
        {
            _swapIndexeses = _swapIndexeses ?? MakeSwapIndexes();

            var count = base.GetValues(values);

            // Rearrange values array to match EntityInfo metadata
            foreach (var swap in _swapIndexeses)
            {
                Swap(values, swap.Index1, swap.Index2);
            }

            return count;
        }

        private static void Swap(IList<object> values, int index1, int index2)
        {
            var obj = values[index2];

            values[index2] = values[index1];
            values[index1] = obj;
        }

        private IEnumerable<SwapIndexes> MakeSwapIndexes()
        {
            IList<SwapIndexes> swapIndexeses = new List<SwapIndexes>();
            var schemaTable = _executeReader.GetSchemaTable();
            var columns = _entityInfo.Columns.Select(e => e.ColumnName).ToList();

            for (var i = 0; i < schemaTable.Rows.Count; i++)
            {
                var columnName = schemaTable.Rows[i].ItemArray[0].ToString();

                if (!_entityInfo.Columns[i].ColumnName.Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                {
                    var index = columns.IndexOf(columnName);

                    if (index != -1 && !swapIndexeses.Any(e => e.Index1 == i && e.Index2 == index || e.Index2 == i && e.Index1 == index))
                    {
                        swapIndexeses.Add(new SwapIndexes
                        {
                            Index1 = i,
                            Index2 = index
                        });
                    }
                }
            }

            return swapIndexeses.ToList();
        }

        public override int FieldCount => _entityInfo.Columns.Count;
    }
}