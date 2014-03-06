using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualObjects.CRUD.Operations
{
    class InsertOperation : UpdateOperation
    {
        public InsertOperation(string commandText, IEntityInfo entityInfo) 
            : base(commandText, entityInfo)
        {
        }

        protected override object Execute(IConnection connection, object entityModel, IEntityInfo entityInfo, string commandText, IDictionary<string, IOperationParameter> parameters, SessionContext sessionContext)
        {
            if (entityInfo.Identity == null)
            {
                return base.Execute(connection, entityModel, entityInfo, commandText, parameters, sessionContext);
            }
            
            var id = connection.ExecuteScalar(commandText, parameters);
            
            UpdateIdentityField(entityModel, entityInfo, id);
            UpdateVersionControlField(entityModel, entityInfo, sessionContext);

            return entityModel;
        }

        private static void UpdateIdentityField(object entityModel, IEntityInfo entityInfo, object id)
        {
            var idValue = Convert.ChangeType(id, entityInfo.Identity.Property.PropertyType);

            entityInfo.Identity.SetFieldFinalValue(entityModel, idValue);
        }

        protected override IEnumerable<IEntityColumnInfo> GetParameters(IEntityInfo entityInfo)
        {
            return entityInfo.Columns
                .Where(e => !e.IsIdentity)
                .Where(e => !e.IsComputed);
        }
    }
}