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
        String InnerJoin { get; }
        String On { get; }
        String IsNull { get; }
        String IsNotNull { get; }
        String OrderBy { get; }
        String Descending{ get; }
        String Count { get; }
        String Sum { get; }
        String Avg { get; }
        String Min { get; }
        String Max { get; }
        String Any { get; }
        String GroupBy { get; }
        String Distinct { get; }

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
        
        String FormatDayOf(string columnName, int index);
        String FormatDayOfYearOf(string columnName, int index);
        String FormatDayOfWeekOf(string columnName, int index);
        String FormatSecondOf(string columnName, int index);
        String FormatHourOf(string columnName, int index);
        String FormatMonthOf(string columnName, int index);
        String FormatYearOf(string columnName, int index);
        String FormatMinuteOf(string columnName, int index);
        String FormatLengthWith(string columnName, int index);
        String BeginMethodCall(string methodCalled);
        String EndMethodCall(string methodCalled);
        String FormatConstant(object parseValue);
    }
}