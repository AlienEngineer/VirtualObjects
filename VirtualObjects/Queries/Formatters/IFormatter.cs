using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VirtualObjects.Config;

namespace VirtualObjects.Queries.Formatters
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFormatter
    {
        /// <summary>
        /// Formats the field with table.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        String FormatFieldWithTable(String name, int index);

        /// <summary>
        /// Formats the name of the table.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        String FormatTableName(String name, int index);

        /// <summary>
        /// Gets the field separator.
        /// </summary>
        /// <value>
        /// The field separator.
        /// </value>
        String FieldSeparator { get; }
        /// <summary>
        /// Gets the select.
        /// </summary>
        /// <value>
        /// The select.
        /// </value>
        String Select { get; }
        /// <summary>
        /// Gets from.
        /// </summary>
        /// <value>
        /// From.
        /// </value>
        String From { get; }
        /// <summary>
        /// Gets the where.
        /// </summary>
        /// <value>
        /// The where.
        /// </value>
        String Where { get; }
        /// <summary>
        /// Gets the and.
        /// </summary>
        /// <value>
        /// The and.
        /// </value>
        String And { get; }
        /// <summary>
        /// Gets the in.
        /// </summary>
        /// <value>
        /// The in.
        /// </value>
        String In { get; }
        /// <summary>
        /// Gets the inner join.
        /// </summary>
        /// <value>
        /// The inner join.
        /// </value>
        String InnerJoin { get; }
        /// <summary>
        /// Gets the on.
        /// </summary>
        /// <value>
        /// The on.
        /// </value>
        String On { get; }
        /// <summary>
        /// Gets the is null.
        /// </summary>
        /// <value>
        /// The is null.
        /// </value>
        String IsNull { get; }
        /// <summary>
        /// Gets the is not null.
        /// </summary>
        /// <value>
        /// The is not null.
        /// </value>
        String IsNotNull { get; }
        /// <summary>
        /// Gets the order by.
        /// </summary>
        /// <value>
        /// The order by.
        /// </value>
        String OrderBy { get; }
        /// <summary>
        /// Gets the descending.
        /// </summary>
        /// <value>
        /// The descending.
        /// </value>
        String Descending{ get; }
        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        String Count { get; }
        String Sum { get; }
        String Avg { get; }
        String Min { get; }
        String Max { get; }
        String Any { get; }
        String GroupBy { get; }
        String Distinct { get; }
        String Union { get; }
        String Delete { get; }
        String Values { get; }
        String Insert { get; }
        String Update { get; }
        String Set { get; }
        String Identity { get; }

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
        String FormatMillisecondOf(string columnName, int index);
        String FormatHourOf(string columnName, int index);
        String FormatMonthOf(string columnName, int index);
        String FormatYearOf(string columnName, int index);
        String FormatMinuteOf(string columnName, int index);
        String FormatDateOf(string columnName, int index);
        String FormatLengthWith(string columnName, int index);
        String BeginMethodCall(string methodCalled);
        String EndMethodCall(string methodCalled);
        String FormatConstant(object parseValue);
        String FormatTableName(string entityName);
        string FormatRowNumber(String orderBy, int index);

        
    }
}