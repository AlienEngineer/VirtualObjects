using System;

namespace VirtualObjects.Config
{
    class EntityDateTimeColumnInfo : EntityColumnInfo
    {
        public override Object SetFieldFinalValue(object entity, object value)
        {
            if ( value == DBNull.Value )
            {
                value = null;
            }

            return SetValue(entity, value ?? default(DateTime));
        }

        public override object GetFieldFinalValue(object entity)
        {
            var value = (DateTime)base.GetFieldFinalValue(entity);

            if (value.Year < 1753)
            {
                return null;
            }

            return value;
        }
    }
}