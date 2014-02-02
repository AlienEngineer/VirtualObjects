using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtualObjects.Config;

namespace VirtualObjects.Core.CRUD.Operations
{
    abstract class Operation : IOperation
    {
        private readonly IEntityInfo _entityInfo;
        private IDictionary<String, Object> _parameters;
        private object _entityModel;

        protected Operation(String commandText, IEntityInfo entityInfo)
        {
            CommandText = commandText;
            _entityInfo = entityInfo;
        }

        public string CommandText { get; private set; }

        public object Execute(IConnection connection)
        {
            return Execute(connection, _entityModel, _entityInfo, CommandText, _parameters);
        }

        public IOperation PrepareOperation(object entityModel)
        {
            _entityModel = entityModel;
            _parameters = GetParameters(_entityInfo)
                .Select(e => new { Key = e.ColumnName, Value = e.GetFieldFinalValue(entityModel) })
                .ToDictionary(e => e.Key, e => e.Value);

            return this;
        }

        protected abstract object Execute(IConnection connection, object entityModel, IEntityInfo entityInfo, string commandText, IDictionary<string, object> parameters);
        protected abstract IEnumerable<IEntityColumnInfo> GetParameters(IEntityInfo entityInfo);
    }
}
