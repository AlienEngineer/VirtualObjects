using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VirtualObjects.Config;
using VirtualObjects.Exceptions;
using VirtualObjects.Queries.ConcurrentReader;
using DataRow = VirtualObjects.Queries.ConcurrentReader.DataRow;

namespace VirtualObjects.Queries.Mapping
{
    class ParallelEntitiesMapper : IEntitiesMapper
    {
        class Mapping
        {
            public DataRow dataRow;
            public Object entity;
            public ManualResetEventSlim mres = new ManualResetEventSlim();
        }

        private readonly IMapper _mapper;
        private readonly IEntityProvider _entityProvider;
        private readonly IEnumerable<IEntityMapper> _entityMappers;

        public ParallelEntitiesMapper(IMapper mapper, IEntityProvider entityProvider, IEnumerable<IEntityMapper> entityMappers)
        {
            _mapper = mapper;
            _entityProvider = entityProvider;
            _entityMappers = entityMappers;
        }

        public IEnumerable<TEntity> MapEntities<TEntity>(IDataReader reader, IQueryInfo queryInfo, SessionContext sessionContext)
        {
            reader = new BlockingDataReader(reader);

            return MapEntities(reader, queryInfo, typeof(TEntity), sessionContext).Cast<TEntity>();
        }

        public IEnumerable<object> MapEntities(IDataReader reader, IQueryInfo queryInfo, Type outputType, SessionContext sessionContext)
        {
            var result = new List<Mapping>();

            var entityInfo = _mapper.Map(outputType);

            var context = new MapperContext
            {
                EntityInfo = entityInfo,
                OutputType = outputType,
                EntityProvider = _entityProvider,
                Mapper = _mapper,
                QueryInfo = queryInfo
            };

            if (entityInfo != null && entityInfo.EntityProvider != null)
            {
                context.EntityProvider = entityInfo.EntityProvider;
            }

            var entityMapper =
                (entityInfo != null && entityInfo.EntityMapper != null) ?
                    entityInfo.EntityMapper :
                    _entityMappers.FirstOrDefault(e => e.CanMapEntity(context));

            if (entityMapper == null)
            {
                throw new MappingException(Errors.Mapping_OutputTypeNotSupported, context);
            }

            try
            {
                //
                // This line enables about 50% more code efficiency.
                //
                _entityProvider.PrepareProvider(context.OutputType, sessionContext);
                entityMapper.PrepareMapper(context);

                if (reader.Read())
                {
                    var columns = reader.GetColumnNames();

                    do
                    {
                        var mapping = new Mapping
                        {
                            dataRow = new DataRow(0)
                            {
                                ColumnNames = columns,
                                Values = reader.GetValues()
                            }
                        };

                        BeginCreateEntity(context, mapping, entityMapper);

                        result.Add(mapping);

                    } while (context.Read || reader.Read());
                }

                return result.Select(e =>
                {
                    e.mres.Wait();
                
                    return e.entity;
                });
            }
            catch (Exception ex)
            {
                if (ex is MappingException)
                {
                    throw;
                }

                throw new MappingException("Unable to map the query into [{Name}]", outputType, ex);
            }
            finally
            {
                reader.Close();
            }
        }

        private void BeginCreateEntity(MapperContext context, Mapping mapping, IEntityMapper entityMapper)
        {
            Task.Factory.StartNew(() =>
            {
                var entity = context.CreateEntity();

                mapping.entity = entity;

                var reader = new RowReader(mapping.dataRow);

                entityMapper.MapEntity(reader, entity, context);

                mapping.mres.Set();
            });
        }

    }
}
