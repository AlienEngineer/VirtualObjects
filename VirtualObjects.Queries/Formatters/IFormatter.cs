using System;
using System.Linq.Expressions;

namespace VirtualObjects.Queries.Formatters
{
    public interface IFormatter
    {
        String FormatFieldWithTable(String name, int index);

        String FormatTableName(String name, int index);

        String FieldSeparator { get; }

        String FormatField(String name);
        
        String FormatNode(ExpressionType nodeType);
        
        string FormatConstant(object value, int count);
    }
}