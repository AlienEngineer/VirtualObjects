using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fasterflect;

namespace VirtualObjects.Config
{
    class EntityColumnInfo : IEntityColumnInfo
    {
        private PropertyInfo _property;

        public int Index { get; set; }

        public string ColumnName { get; set; }

        public bool IsKey { get; set; }

        public bool IsIdentity { get; set; }

        public PropertyInfo Property
        {
            get { return _property; }
            set
            {
                _property = value;
                if ( _property.PropertyType.IsValueType )
                {
                    DefaultValue = _property.PropertyType.CreateInstance();
                }
            }
        }

        public IEntityInfo EntityInfo { get; set; }

        public IEntityColumnInfo ForeignKey { get; set; }

        public List<KeyValuePair<IEntityColumnInfo, IEntityColumnInfo>> ForeignKeyLinks { get; set; }

        public Func<object, object> ValueGetter { get; set; }

        public Action<object, object> ValueSetter { get; set; }

        public string[] Formats { get; set; }

        public object SetValue(object entity, object value)
        {
            if ( value == DBNull.Value )
            {
                value = null;

                if ( Property.PropertyType.IsValueType )
                {
                    value = DefaultValue;
                }
            }

            ValueSetter(entity, value);
            return null;
        }

        public object GetValue(object entity)
        {
            return ValueGetter(entity);
        }

        public virtual object GetFieldFinalValue(object entity)
        {
            return GetValue(entity);
        }

        public virtual object SetFieldFinalValue(object entity, object value)
        {
            return SetValue(entity, value);
        }

        public virtual string BindOrName { get { return ColumnName; } }
        public bool IsVersionControl { get; set; }
        public bool IsComputed { get; set; }

        public object DefaultValue { get; private set; }
        public bool InjectNulls { get; set; }
        public bool HasFormattingStyles { get { return Formats != null && Formats.Any(); } }

        public virtual IEntityColumnInfo GetLastBind() { return this; }

        public override string ToString()
        {
            if (EntityInfo != null)
            {
                return EntityInfo.EntityName + " : " + ColumnName;
            }

            return base.ToString();
        }
    }
}