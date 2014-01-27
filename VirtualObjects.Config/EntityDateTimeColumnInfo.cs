using System;

namespace VirtualObjects.Config
{
    class EntityDateTimeColumnInfo : EntityColumnInfo
    {
        public override void SetFieldFinalValue(object entity, object value)
        {
            if ( value == DBNull.Value )
            {
                value = null;
            }

            SetValue(entity, value ?? default(DateTime));
        }
    }
}