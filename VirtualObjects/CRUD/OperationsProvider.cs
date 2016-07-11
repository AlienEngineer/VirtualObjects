using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VirtualObjects.CRUD.Operations;
using VirtualObjects.Queries;
using VirtualObjects.Queries.Formatters;
using VirtualObjects.Queries.Mapping;

namespace VirtualObjects.CRUD
{
    class OperationsProvider : IOperationsProvider
    {
        private readonly IFormatter _formatter;
        private readonly IEntityMapper _mapper;
        private readonly IEntityProvider _entityProvider;

        public OperationsProvider(IFormatter formatter, IEntityMapper mapper, IEntityProvider entityProvider)
        {
            _formatter = formatter;
            _mapper = mapper;
            _entityProvider = entityProvider;
        }

        public IOperations CreateOperations(IEntityInfo entityInfo)
        {
            return new Operations.Operations
            {
                DeleteOperation = CreateDeleteOperation(entityInfo),
                GetOperation = CreateGetOperation(entityInfo),
                InsertOperation = CreateInsertOperation(entityInfo),
                UpdateOperation = CreateUpdateOperation(entityInfo),
                GetVersionOperation = CreateGetVersionOperation(entityInfo),
                CountOperation = CreateCountOperation(entityInfo),
                QueryOperation = new QueryOperation(entityInfo)
            };
        }

        private IOperation CreateCountOperation(IEntityInfo entityInfo)
        {
            StringBuffer text = _formatter.Select;
            text += " ";
            text += _formatter.Count;
            text += " ";
            text += _formatter.From;
            text += " ";
            text += _formatter.FormatTableName(entityInfo.EntityName);

            return new CountOperation(text, entityInfo);
        }

        private IOperation CreateUpdateOperation(IEntityInfo entityInfo)
        {
            StringBuffer text = _formatter.Update;
            text += " ";
            text += _formatter.FormatTableName(entityInfo.EntityName);
            text += " ";
            text += _formatter.Set;
            text += " ";

            foreach ( var column in entityInfo.Columns.Where(e => !e.IsKey && !e.IsVersionControl) )
            {
                AppendEquality(text, column);
                text += _formatter.FieldSeparator;
            }

            text.RemoveLast(_formatter.FieldSeparator);

            text += " ";
            text += _formatter.Where;
            text += " ";

            CreateWhereClause(text, entityInfo);

            return new VersionCheckOperation(new UpdateOperation(text, entityInfo), entityInfo);
        }

        private IOperation CreateInsertOperation(IEntityInfo entityInfo)
        {
            var columns = entityInfo.Columns.Where(e => !e.IsIdentity && !e.IsVersionControl).ToList();

            StringBuffer text = _formatter.Insert;

            text += " ";
            text += _formatter.FormatTableName(entityInfo.EntityName);
            text += " ";

            text += _formatter.BeginWrap();

            text += CreateProjection(columns);
            text += _formatter.EndWrap();

            text += " ";
            text += _formatter.Values;
            text += " ";

            text += _formatter.BeginWrap();
#if NET35
            text += string.Join(_formatter.FieldSeparator,
                                columns.Select(e => "@" + e.ColumnName.Replace(' ', '_')).ToArray());
#else
            text += string.Join(_formatter.FieldSeparator,
                                columns.Select(e => "@" + e.ColumnName.Replace(' ', '_')));
#endif


            text += _formatter.EndWrap();

            if ( entityInfo.Identity != null )
            {
                text += " ";
                text += _formatter.Identity;
            }


            return new InsertOperation(text, entityInfo);
        }

        private IOperation CreateDeleteOperation(IEntityInfo entityInfo)
        {
            StringBuffer text = _formatter.Delete;
            text += " ";
            text += _formatter.From;
            text += " ";
            text += _formatter.FormatTableName(entityInfo.EntityName);
            text += " ";
            text += _formatter.Where;
            text += " ";

            CreateWhereClause(text, entityInfo);

            return new DeleteOperation(text, entityInfo);
        }

        private IOperation CreateGetOperation(IEntityInfo entityInfo)
        {
            StringBuffer text = _formatter.Select;
            text += " ";
            text += CreateProjection(entityInfo.Columns);
            text += " ";
            text += _formatter.From;
            text += " ";
            text += _formatter.FormatTableName(entityInfo.EntityName);
            text += " ";
            text += _formatter.Where;
            text += " ";

            CreateWhereClause(text, entityInfo);

            return new GetOperation(text, entityInfo, _mapper, _entityProvider);
        }

        private IOperation CreateGetVersionOperation(IEntityInfo entityInfo)
        {

            StringBuffer text = _formatter.Select;
            text += " ";
            text += CreateProjection(entityInfo.Columns.Where(e => e.IsVersionControl));
            text += " ";
            text += _formatter.From;
            text += " ";
            text += _formatter.FormatTableName(entityInfo.EntityName);
            text += " ";
            text += _formatter.Where;
            text += " ";

            CreateWhereClause(text, entityInfo);

            return new GetVersionOperation(text, entityInfo);
        }


        private string CreateProjection(IEnumerable<IEntityColumnInfo> columns)
        {
#if NET35
            return string.Join(_formatter.FieldSeparator, columns.Select(e => _formatter.FormatField(e.ColumnName)).ToArray());
#else
            return string.Join(_formatter.FieldSeparator, columns.Select(e => _formatter.FormatField(e.ColumnName)));
#endif
            
        }

        private void CreateWhereClause(StringBuffer text, IEntityInfo entityInfo)
        {
            foreach ( var keyColumn in entityInfo.KeyColumns )
            {
                AppendEquality(text, keyColumn);
                text += " ";
                text += _formatter.And;
                text += " ";
            }

            text.RemoveLast(_formatter.And.Length + 2);
        }

        private void AppendEquality(StringBuffer text, IEntityColumnInfo keyColumn)
        {
            text += _formatter.FormatField(keyColumn.ColumnName);
            text += _formatter.FormatNode(ExpressionType.Equal);
            text += "@" + keyColumn.ColumnName.Replace(' ', '_');
        }
    }
}
