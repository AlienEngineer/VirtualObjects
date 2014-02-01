using System.Linq;
using System.Linq.Expressions;
using VirtualObjects.Config;
using VirtualObjects.Core.CRUD.Operations;
using VirtualObjects.Queries;
using VirtualObjects.Queries.Formatters;

namespace VirtualObjects.Core.CRUD
{
    public interface IOperationsProvider
    {
        IOperations CreateOperations(IEntityInfo entityInfo);
    }

    class OperationsProvider : IOperationsProvider
    {
        private readonly IFormatter _formatter;
        private readonly IEntityMapper _mapper;

        public OperationsProvider(IFormatter formatter, IEntityMapper mapper)
        {
            _formatter = formatter;
            _mapper = mapper;
        }

        public IOperations CreateOperations(IEntityInfo entityInfo)
        {
            return new Operations.Operations
            {
                DeleteOperation = CreateDeleteOperation(entityInfo),
                GetOperation = CreateGetOperation(entityInfo),
                InsertOperation = CreateInsertOperation(entityInfo),
                UpdateOperation = CreateUpdateOperation(entityInfo)
            };
        }

        private IOperation CreateUpdateOperation(IEntityInfo entityInfo)
        {
            return new UpdateOperation(text, entityInfo);
        }

        private IOperation CreateInsertOperation(IEntityInfo entityInfo)
        {

            return new InsertOperation(text, entityInfo);
        }

        private IOperation CreateDeleteOperation(IEntityInfo entityInfo)
        {
            StringBuffer text = _formatter.Delete;
            text += " ";
            text += _formatter.From;
            text += " ";
            text += _formatter.FormatTableName(entityInfo.EntityName, 0);
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
            text += string.Join(_formatter.FieldSeparator, entityInfo.Columns.Select(e => _formatter.FormatField(e.ColumnName)));
            text += " ";
            text += _formatter.From;
            text += " ";
            text += _formatter.FormatTableName(entityInfo.EntityName, 0);
            text += " "; 
            text += _formatter.Where;
            text += " ";
            
            CreateWhereClause(text, entityInfo);

            return new GetOperation(text, entityInfo, _mapper);
        }

        private StringBuffer CreateWhereClause(StringBuffer text, IEntityInfo entityInfo)
        {
            foreach (var keyColumn in entityInfo.KeyColumns)
            {
                text += _formatter.FormatField(keyColumn.ColumnName);
                text += " ";
                text += _formatter.FormatNode(ExpressionType.Equal);
                text += " "; 
                text += "@" + keyColumn.ColumnName.Replace(' ', '_');
                text += _formatter.FieldSeparator;
            }

            text.RemoveLast(_formatter.FieldSeparator);
            return text;
        }

    }
}
