using System;

namespace VirtualObjects.Config
{
    class EntityBoundColumnInfo : EntityColumnInfo
    {
        public override Object GetFieldFinalValue(object entity)
        {
            var value = GetValue(entity);
            return ForeignKey != null && value != null ? ForeignKey.GetValue(value) : value;
        }


        public override object SetFieldFinalValue(object entity, object value)
        {
            if ( ForeignKey != null )
            {
                if ( value == DBNull.Value )
                {
                    return null;
                }

                var instance =  Activator.CreateInstance(ForeignKey.EntityInfo.EntityType);
                SetValue(entity, instance);

                ForeignKey.SetFieldFinalValue(instance, value);

                return instance;
            }
            return null;
        }

        public override string BindOrName
        {
            get
            {
                return ForeignKey != null ? ForeignKey.ColumnName : ColumnName;
            }
        }

        public override IEntityColumnInfo GetLastBind()
        {
            return ForeignKey.GetLastBind();
        }
    }
}