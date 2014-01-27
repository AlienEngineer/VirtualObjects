using System;
using System.Reflection;

namespace VirtualObjects.Config
{
    public interface IEntityColumnInfo
    {
        String ColumnName { get; }

        Boolean IsKey { get; }

        Boolean IsIdentity { get; }

        PropertyInfo Property { get; }

        IEntityInfo EntityInfo { get;  }
        
        IEntityColumnInfo ForeignKey { get; set; }

        string BindOrName { get; }

        Func<Object, Object> ValueGetter { get; }

        Action<Object, Object> ValueSetter { get; }

        void SetValue(Object entity, Object value);

        object GetValue(Object entity);

        object GetFieldFinalValue(object entity);

        void SetFieldFinalValue(object entity, object value);

        IEntityColumnInfo GetLastBind();
        
        
    }
}