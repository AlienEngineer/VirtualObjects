using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VirtualObjects.Config;

namespace VirtualObjects.Queries.Formatters
{
    public interface IFormatter
    {
        String FormatFieldWithTable(String name, int index);

        String FormatTableName(String name, int index);

        String FieldSeparator { get; }
        String Select { get; }
        String From { get; }
        String Where { get; }
        String And { get; }
        String In { get; }

        String FormatField(String name);

        String FormatNode(ExpressionType nodeType);

        String FormatConstant(object value, int count);

        String FormatGetDate();

        String FormatTakeN(int take);

        string FormatRowNumber(IEnumerable<IEntityColumnInfo> keyColumns, int index);

        string FormatFields(IEnumerable<IEntityColumnInfo> columns, int index);

        string GetRowNumberField(int index);

        string GetTableAlias(int index);

        String BeginWrap();

        string EndWrap(int i = 1);
    }
}