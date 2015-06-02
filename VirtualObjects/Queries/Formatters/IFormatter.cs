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
        string FormatFieldWithTable(string name, int index);

        /// <summary>
        /// Formats the name of the table.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        string FormatTableName(string name, int index);

        /// <summary>
        /// Gets the field separator.
        /// </summary>
        /// <value>
        /// The field separator.
        /// </value>
        string FieldSeparator { get; }
        /// <summary>
        /// Gets the select.
        /// </summary>
        /// <value>
        /// The select.
        /// </value>
        string Select { get; }
        /// <summary>
        /// Gets from.
        /// </summary>
        /// <value>
        /// From.
        /// </value>
        string From { get; }
        /// <summary>
        /// Gets the where.
        /// </summary>
        /// <value>
        /// The where.
        /// </value>
        string Where { get; }
        /// <summary>
        /// Gets the and.
        /// </summary>
        /// <value>
        /// The and.
        /// </value>
        string And { get; }
        /// <summary>
        /// Gets the in.
        /// </summary>
        /// <value>
        /// The in.
        /// </value>
        string In { get; }
        /// <summary>
        /// Gets the inner join.
        /// </summary>
        /// <value>
        /// The inner join.
        /// </value>
        string InnerJoin { get; }
        /// <summary>
        /// Gets the on.
        /// </summary>
        /// <value>
        /// The on.
        /// </value>
        string On { get; }
        /// <summary>
        /// Gets the is null.
        /// </summary>
        /// <value>
        /// The is null.
        /// </value>
        string IsNull { get; }
        /// <summary>
        /// Gets the is not null.
        /// </summary>
        /// <value>
        /// The is not null.
        /// </value>
        string IsNotNull { get; }
        /// <summary>
        /// Gets the order by.
        /// </summary>
        /// <value>
        /// The order by.
        /// </value>
        string OrderBy { get; }
        /// <summary>
        /// Gets the descending.
        /// </summary>
        /// <value>
        /// The descending.
        /// </value>
        string Descending{ get; }
        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        string Count { get; }
        /// <summary>
        /// Gets the sum.
        /// </summary>
        /// <value>
        /// The sum.
        /// </value>
        string Sum { get; }
        /// <summary>
        /// Gets the average.
        /// </summary>
        /// <value>
        /// The average.
        /// </value>
        string Avg { get; }
        /// <summary>
        /// Gets the minimum.
        /// </summary>
        /// <value>
        /// The minimum.
        /// </value>
        string Min { get; }
        /// <summary>
        /// Gets the maximum.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        string Max { get; }
        /// <summary>
        /// Gets any.
        /// </summary>
        /// <value>
        /// Any.
        /// </value>
        string Any { get; }
        /// <summary>
        /// Gets the group by.
        /// </summary>
        /// <value>
        /// The group by.
        /// </value>
        string GroupBy { get; }
        /// <summary>
        /// Gets the distinct.
        /// </summary>
        /// <value>
        /// The distinct.
        /// </value>
        string Distinct { get; }
        /// <summary>
        /// Gets the union.
        /// </summary>
        /// <value>
        /// The union.
        /// </value>
        string Union { get; }
        /// <summary>
        /// Gets the delete.
        /// </summary>
        /// <value>
        /// The delete.
        /// </value>
        string Delete { get; }
        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        string Values { get; }
        /// <summary>
        /// Gets the insert.
        /// </summary>
        /// <value>
        /// The insert.
        /// </value>
        string Insert { get; }
        /// <summary>
        /// Gets the update.
        /// </summary>
        /// <value>
        /// The update.
        /// </value>
        string Update { get; }
        /// <summary>
        /// Gets the set.
        /// </summary>
        /// <value>
        /// The set.
        /// </value>
        string Set { get; }
        /// <summary>
        /// Gets the identity.
        /// </summary>
        /// <value>
        /// The identity.
        /// </value>
        string Identity { get; }

        /// <summary>
        /// Gets the left join.
        /// </summary>
        /// <value>
        /// The left join.
        /// </value>
        string LeftJoin { get; }

        /// <summary>
        /// Formats the field.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        string FormatField(string name);

        /// <summary>
        /// Formats the node.
        /// </summary>
        /// <param name="nodeType">Type of the node.</param>
        /// <returns></returns>
        string FormatNode(ExpressionType nodeType);

        /// <summary>
        /// Formats the constant.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        string FormatConstant(object value, int count);

        /// <summary>
        /// Formats the get date.
        /// </summary>
        /// <returns></returns>
        string FormatGetDate();

        /// <summary>
        /// Formats the take n.
        /// </summary>
        /// <param name="take">The take.</param>
        /// <returns></returns>
        string FormatTakeN(int take);

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
        string BeginWrap();

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
        string FormatDayOf(string columnName, int index);
        /// <summary>
        /// Formats the day of year of.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        string FormatDayOfYearOf(string columnName, int index);
        /// <summary>
        /// Formats the day of week of.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        string FormatDayOfWeekOf(string columnName, int index);
        /// <summary>
        /// Formats the second of.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        string FormatSecondOf(string columnName, int index);
        /// <summary>
        /// Formats the millisecond of.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        string FormatMillisecondOf(string columnName, int index);
        /// <summary>
        /// Formats the hour of.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        string FormatHourOf(string columnName, int index);
        /// <summary>
        /// Formats the month of.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        string FormatMonthOf(string columnName, int index);
        /// <summary>
        /// Formats the year of.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        string FormatYearOf(string columnName, int index);
        /// <summary>
        /// Formats the minute of.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        string FormatMinuteOf(string columnName, int index);
        /// <summary>
        /// Formats the date of.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        string FormatDateOf(string columnName, int index);
        /// <summary>
        /// Formats the length with.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        string FormatLengthWith(string columnName, int index);
        /// <summary>
        /// Begins the method call.
        /// </summary>
        /// <param name="methodCalled">The method called.</param>
        /// <returns></returns>
        string BeginMethodCall(string methodCalled);
        /// <summary>
        /// Ends the method call.
        /// </summary>
        /// <param name="methodCalled">The method called.</param>
        /// <returns></returns>
        string EndMethodCall(string methodCalled);
        /// <summary>
        /// Formats the constant.
        /// </summary>
        /// <param name="parseValue">The parse value.</param>
        /// <returns></returns>
        string FormatConstant(object parseValue);
        /// <summary>
        /// Formats the name of the table.
        /// </summary>
        /// <param name="entityName">Name of the entity.</param>
        /// <returns></returns>
        string FormatTableName(string entityName);
        /// <summary>
        /// Formats the row number.
        /// </summary>
        /// <param name="orderBy">The order by.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        string FormatRowNumber(string orderBy, int index);

        /// <summary>
        /// Formats to lower with.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        string FormatToLowerWith(string columnName, int index);

        /// <summary>
        /// Formats to upper with.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        string FormatToUpperWith(string columnName, int index);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <returns></returns>
        bool SupportsCustomFunction(string methodName);
    }
}