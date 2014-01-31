using System.Collections.Generic;
using System.Linq;
using VirtualObjects.Config;

namespace VirtualObjects.Core.CRUD.Operations
{
    class InsertOperation : Operation
    {
        public InsertOperation(string commandText, IEntityInfo entityInfo) 
            : base(commandText, entityInfo)
        {
        }

        protected override object Execute(IConnection connection, object entityModel, IEntityInfo entityInfo, string commandText, IDictionary<string, object> parameters)
        {
            if (entityInfo.Identity == null)
            {
                connection.ExecuteNonQuery(commandText, parameters);
            }
            else
            {
                var id = connection.ExecuteScalar(commandText, parameters);    

                entityInfo.Identity.SetFieldFinalValue(entityModel, id);
            }

            return entityModel;
        }

        protected override IEnumerable<IEntityColumnInfo> GetParameters(IEntityInfo entityInfo)
        {
            return entityInfo.Columns.Where(e => !e.IsIdentity);
        }
    }
}