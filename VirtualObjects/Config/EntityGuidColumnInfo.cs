using System;

namespace VirtualObjects.Config
{
    class EntityGuidColumnInfo : EntityColumnInfo
    {
        public override object SetFieldFinalValue(object entity, object value)
        {
            if ( value == DBNull.Value )
            {
                value = null;
            }

            return SetValue(entity, value ?? default(Guid));
        }

        public override object GetFieldFinalValue(object entity)
        {
            var value = (Guid)base.GetFieldFinalValue(entity);

            if ( Guid.Empty == value )
            {
                return null;
            }

            return value;
        }
    }
}