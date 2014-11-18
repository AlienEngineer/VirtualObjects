using System;
using System.Reflection;
using System.Collections.Generic;
using VirtualObjects.Reflection;

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

        public Func<Object, Object> ValueGetter { get; set; }

        public Action<Object, Object> ValueSetter { get; set; }

        public Object SetValue(Object entity, Object value)
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

        public object GetValue(Object entity)
        {
            return ValueGetter(entity);
        }

        public virtual object GetFieldFinalValue(object entity)
        {
            return GetValue(entity);
        }

        public virtual Object SetFieldFinalValue(object entity, object value)
        {
            return SetValue(entity, value);
        }

        public virtual string BindOrName { get { return ColumnName; } }
        public bool IsVersionControl { get; set; }
        public bool IsComputed { get; set; }

        public object DefaultValue { get; private set; }
        public bool InjectNulls { get; set; }

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