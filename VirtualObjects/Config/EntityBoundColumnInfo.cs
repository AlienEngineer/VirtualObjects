using System;
using Fasterflect;

namespace VirtualObjects.Config
{
    class EntityBoundColumnInfo : EntityColumnInfo
    {
        public override Object GetFieldFinalValue(object entity)
        {
            var value = GetValue(entity);
            return ForeignKey != null && value != null ? ForeignKey.GetValue(value) : value;
        }


        public override void SetFieldFinalValue(object entity, object value)
        {
            if ( ForeignKey != null )
            {
                if ( value == DBNull.Value )
                {
                    return;
                }

                var instance = ForeignKey.EntityInfo.EntityType.CreateInstance();
                SetValue(entity, instance);

                ForeignKey.SetFieldFinalValue(instance, value);
            }
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