using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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
        /// <summary>
        /// Gets the sum.
        /// </summary>
        /// <value>
        /// The sum.
        /// </value>
        String Sum { get; }
        /// <summary>
        /// Gets the average.
        /// </summary>
        /// <value>
        /// The average.
        /// </value>
        String Avg { get; }
        /// <summary>
        /// Gets the minimum.
        /// </summary>
        /// <value>
        /// The minimum.
        /// </value>
        String Min { get; }
        /// <summary>
        /// Gets the maximum.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        String Max { get; }
        /// <summary>
        /// Gets any.
        /// </summary>
        /// <value>
        /// Any.
        /// </value>
        String Any { get; }
        /// <summary>
        /// Gets the group by.
        /// </summary>
        /// <value>
        /// The group by.
        /// </value>
        String GroupBy { get; }
        /// <summary>
        /// Gets the distinct.
        /// </summary>
        /// <value>
        /// The distinct.
        /// </value>
        String Distinct { get; }
        /// <summary>
        /// Gets the union.
        /// </summary>
        /// <value>
        /// The union.
        /// </value>
        String Union { get; }
        /// <summary>
        /// Gets the delete.
        /// </summary>
        /// <value>
        /// The delete.
        /// </value>
        String Delete { get; }
        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        String Values { get; }
        /// <summary>
        /// Gets the insert.
        /// </summary>
        /// <value>
        /// The insert.
        /// </value>
        String Insert { get; }
        /// <summary>
        /// Gets the update.
        /// </summary>
        /// <value>
        /// The update.
        /// </value>
        String Update { get; }
        /// <summary>
        /// Gets the set.
        /// </summary>
        /// <value>
        /// The set.
        /// </value>
        String Set { get; }
        /// <summary>
        /// Gets the identity.
        /// </summary>
        /// <value>
        /// The identity.
        /// </value>
        String Identity { get; }

        /// <summary>
        /// Gets the left join.
        /// </summary>
        /// <value>
        /// The left join.
        /// </value>
        String LeftJoin { get; }

        /// <summary>
        /// Formats the field.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        String FormatField(String name);

        /// <summary>
        /// Formats the node.
        /// </summary>
        /// <param name="nodeType">Type of the node.</param>
        /// <returns></returns>
        String FormatNode(ExpressionType nodeType);

        /// <summary>
        /// Formats the constant.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        String FormatConstant(object value, int count);

        /// <summary>
        /// Formats the get date.
        /// </summary>
        /// <returns></returns>
        String FormatGetDate();

        /// <summary>
        /// Formats the take n.
        /// </summary>
        /// <param name="take">The take.</param>
        /// <returns></returns>
        String FormatTakeN(int take);

        /// <summary>
        /// Formats the row number.
        /// </summary>
        /// <param name="keyColumns">The key columns.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        string FormatRowNumber(IEnumerable<IEntityColumnInfo> keyColumns, int index);

        /// <summary>
        /// Formats the fields.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        string FormatFields(IEnumerable<IEntityColumnInfo> columns, int index);

        /// <summary>
        /// Gets the row number field.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        string GetRowNumberField(int index);

        /// <summary>
        /// Gets the table alias.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        string GetTableAlias(int index);

        /// <summary>
        /// Begins the wrap.
        /// </summary>
        /// <returns></returns>
        String BeginWrap();

        /// <summary>
        /// Ends the wrap.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns></returns>
        string EndWrap(int i = 1);

        /// <summary>
        /// Formats the day of.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        String FormatDayOf(string columnName, int index);
        /// <summary>
        /// Formats the day of year of.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        String FormatDayOfYearOf(string columnName, int index);
        /// <summary>
        /// Formats the day of week of.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        String FormatDayOfWeekOf(string columnName, int index);
        /// <summary>
        /// Formats the second of.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        String FormatSecondOf(string columnName, int index);
        /// <summary>
        /// Formats the millisecond of.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        String FormatMillisecondOf(string columnName, int index);
        /// <summary>
        /// Formats the hour of.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        String FormatHourOf(string columnName, int index);
        /// <summary>
        /// Formats the month of.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        String FormatMonthOf(string columnName, int index);
        /// <summary>
        /// Formats the year of.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        String FormatYearOf(string columnName, int index);
        /// <summary>
        /// Formats the minute of.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        String FormatMinuteOf(string columnName, int index);
        /// <summary>
        /// Formats the date of.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        String FormatDateOf(string columnName, int index);
        /// <summary>
        /// Formats the length with.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        String FormatLengthWith(string columnName, int index);
        /// <summary>
        /// Begins the method call.
        /// </summary>
        /// <param name="methodCalled">The method called.</param>
        /// <returns></returns>
        String BeginMethodCall(string methodCalled);
        /// <summary>
        /// Ends the method call.
        /// </summary>
        /// <param name="methodCalled">The method called.</param>
        /// <returns></returns>
        String EndMethodCall(string methodCalled);
        /// <summary>
        /// Formats the constant.
        /// </summary>
        /// <param name="parseValue">The parse value.</param>
        /// <returns></returns>
        String FormatConstant(object parseValue);
        /// <summary>
        /// Formats the name of the table.
        /// </summary>
        /// <param name="entityName">Name of the entity.</param>
        /// <returns></returns>
        String FormatTableName(string entityName);
        /// <summary>
        /// Formats the row number.
        /// </summary>
        /// <param name="orderBy">The order by.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        string FormatRowNumber(String orderBy, int index);

        /// <summary>
        /// Formats to lower with.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        String FormatToLowerWith(string columnName, int index);

        /// <summary>
        /// Formats to upper with.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        String FormatToUpperWith(string columnName, int index);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        bool SupportsCustomFunction(string methodName);
    }
}