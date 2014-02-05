using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualObjects.CRUD.Operations
{
    class InsertOperation : Operation
    {
        public InsertOperation(string commandText, IEntityInfo entityInfo) 
            : base(commandText, entityInfo)
        {
        }

        protected override object Execute(IConnection connection, object entityModel, IEntityInfo entityInfo, string commandText, IDictionary<string, IOperationParameter> parameters, SessionContext sessionContext)
        {
            if (entityInfo.Identity == null)
            {
                connection.ExecuteNonQuery(commandText, parameters);
            }
            else
            {
                var id = connection.ExecuteScalar(commandText, parameters);
                var idValue = Convert.ChangeType(id, entityInfo.Identity.Property.PropertyType);

                entityInfo.Identity.SetFieldFinalValue(entityModel, idValue);
            }

            return entityModel;
        }

        protected override IEnumerable<IEntityColumnInfo> GetParameters(IEntityInfo entityInfo)
        {
            return entityInfo.Columns.Where(e => !e.IsIdentity);
        }
    }
}