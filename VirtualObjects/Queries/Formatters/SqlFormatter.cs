﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using VirtualObjects.Exceptions;
using VirtualObjects.Queries.Translation;

namespace VirtualObjects.Queries.Formatters
{
    class SqlFormatter : IFormatter
    {
        private readonly ICustomFunctionTranslation _functionTranslation;
        private const string SEPARATOR = ", ";
        private const string TABLE_PREFIX = "T";

        public SqlFormatter(ICustomFunctionTranslation functionTranslation)
        {
            _functionTranslation = functionTranslation ?? new CustomSqlFunctionTranslation();
            Select = "Select";
            From = "From";
            Where = "Where";
            And = "And";
            In = "In";
            On = "On";
            IsNull = "Is Null";
            IsNotNull = "Is Not Null";
            InnerJoin = "Inner Join";
            LeftJoin = "Left Join";
            OrderBy = "Order By";
            Descending = "Desc";
            Count = "Count(*)";
            Any = "Case When Count(*) > 0 Then 1 When Count(*) = 0 Then 0 End";
            Sum = "Sum";
            Avg = "Avg";
            Min = "Min";
            Max = "Max";
            GroupBy = "Group By";
            Distinct = "Distinct";
            Union = "Union All";
            Delete = "Delete";
            Insert = "Insert Into";
            Values = "Values";
            Update = "Update";
            Set = "Set";
            Identity = "Select @@IDENTITY";
        }

        protected static string Wrap(string name)
        {
            var result = new StringBuffer();

            foreach ( var item in name.Split('.') )
            {
                result += string.Format("[{0}]", item);
                result += ".";
            }

            result.RemoveLast(".");
            return result;
        }

        public String FieldSeparator
        {
            get { return SEPARATOR; }
        }

        public string Select { get; private set; }
        public string From { get; private set; }
        public string Where { get; private set; }
        public string And { get; private set; }
        public string In { get; private set; }
        public string InnerJoin { get; private set; }
        public string On { get; private set; }
        public string IsNull { get; private set; }
        public string IsNotNull { get; private set; }
        public string OrderBy { get; private set; }
        public string Descending { get; private set; }
        public string Count { get; private set; }
        public string Sum { get; private set; }
        public string Avg { get; private set; }
        public string Min { get; private set; }
        public string Max { get; private set; }
        public string Any { get; private set; }
        public string GroupBy { get; private set; }
        public string Distinct { get; private set; }
        public string Union { get; private set; }
        public string Delete { get; private set; }
        public string Values { get; private set; }
        public string Insert { get; private set; }
        public string Update { get; private set; }
        public string Set { get; private set; }
        public string Identity { get; private set; }
        public string LeftJoin { get; private set; }

        public String FormatField(String name)
        {
            return Wrap(name);
        }

        public String FormatFieldWithTable(String name, int index)
        {
            return string.Format("{0}.{1}", GetTableAlias(index), Wrap(name));
        }

        public virtual String FormatTableName(String name, int index)
        {
            return string.Format("{0} {1}", Wrap(name), GetTableAlias(index));
        }

        public string FormatNode(ExpressionType nodeType)
        {
            switch ( nodeType )
            {
                case ExpressionType.Add: return " + ";
                case ExpressionType.Subtract: return " - ";
                case ExpressionType.Divide: return " / ";
                case ExpressionType.Multiply: return " * ";
                case ExpressionType.Equal: return " = ";
                case ExpressionType.AndAlso: return " And ";
                case ExpressionType.OrElse: return " Or ";
                case ExpressionType.NotEqual: return " != ";
                case ExpressionType.GreaterThan: return " > ";
                case ExpressionType.GreaterThanOrEqual: return " >= ";
                case ExpressionType.LessThan: return " < ";
                case ExpressionType.LessThanOrEqual: return " <= ";
                default:
                    throw new UnsupportedException(Errors.SQL_UnableToFormatNode, new { NodeType = nodeType });

            }
        }

        public string FormatConstant(object value, int count)
        {
            return "@p" + count;
        }

        public string FormatGetDate()
        {
            return "GetDate()";
        }

        public string FormatTakeN(int take)
        {
            return "TOP " + take;
        }

        public string FormatRowNumber(IEnumerable<IEntityColumnInfo> keyColumns, int index)
        {
            var columns = keyColumns as IList<IEntityColumnInfo> ?? keyColumns.ToList();

            return FormatRowNumber(" Order By " + FormatFields(columns, 100 + index), index);
        }

        public string FormatRowNumber(String orderBy, int index)
        {
            return new StringBuilder()
                .Append("ROW_NUMBER() OVER (")
                .Append(orderBy)
                .Append(") as [Internal_Row_Index], *")
                .ToString();
        }

        public string FormatToLowerWith(string columnName, int index)
        {
            return FormatFunctionCall("ToLower", columnName, index);
        }

        public string FormatToUpperWith(string columnName, int index)
        {
            return FormatFunctionCall("ToUpper", columnName, index);
        }

        public bool SupportsCustomFunction(string methodName)
        {
            return _functionTranslation.SupportsMethod(methodName);
        }

        public string FormatFields(IEnumerable<IEntityColumnInfo> columns, int index)
        {
            return String.Join(
                FieldSeparator,
                columns.Select(e => FormatFieldWithTable(e.ColumnName, index))
            );

        }

        public string GetRowNumberField(int index)
        {
            return FormatFieldWithTable("Internal_Row_Index", index);
        }

        public string GetTableAlias(int index)
        {
            return FormatField(TABLE_PREFIX + index);
        }

        public string BeginWrap()
        {
            return "(";
        }

        public string EndWrap(int i = 1)
        {
            return new string(')', i);
        }

        private String FormatFunctionCall(String function, String columnName, int index)
        {
            var buffer = new StringBuffer();
            buffer += function;
            buffer += BeginWrap();
            buffer += FormatFieldWithTable(columnName, index);
            buffer += EndWrap();

            return buffer;
        }

        private String FormatFunctionCall(String function, String parameters, String columnName, int index)
        {
            var buffer = new StringBuffer();
            buffer += function;
            buffer += BeginWrap();
            buffer += parameters;
            buffer += ", ";
            buffer += FormatFieldWithTable(columnName, index);
            buffer += EndWrap();

            return buffer;
        }

        public string FormatDayOf(string columnName, int index)
        {
            return FormatFunctionCall("Day", columnName, index);
        }

        public String FormatDateOf(string columnName, int index)
        {
            return FormatFunctionCall("Cast", columnName + " as Date", index);
        }

        public string FormatDayOfYearOf(string columnName, int index)
        {
            return FormatFunctionCall("Datepart", "'dy'", columnName, index);
        }

        public string FormatDayOfWeekOf(string columnName, int index)
        {
            return FormatFunctionCall("Datepart", "'dw'", columnName, index);
        }

        public string FormatSecondOf(string columnName, int index)
        {
            return FormatFunctionCall("Datepart", "'s'", columnName, index);
        }

        public string FormatMillisecondOf(string columnName, int index)
        {
            return FormatFunctionCall("Datepart", "'ms'", columnName, index);
        }

        public string FormatHourOf(string columnName, int index)
        {
            return FormatFunctionCall("Datepart", "'h'", columnName, index);
        }

        public string FormatMinuteOf(string columnName, int index)
        {
            return FormatFunctionCall("Datepart", "'m'", columnName, index);
        }

        public string FormatLengthWith(string columnName, int index)
        {
            return FormatFunctionCall("Len", columnName, index);
        }

        public string BeginMethodCall(string methodCalled)
        {
            switch ( methodCalled )
            {
                case "ToString":
                case "ToInt16":
                case "ToInt32":
                case "ToInt64":
                case "ToUInt16":
                case "ToUInt32":
                case "ToUInt64":
                case "ToByte":
                case "ToSByte":
                case "ToDouble":
                case "ToDecimal":
                case "ToBoolean":
                case "ToSingle":
                case "ToDateTime":
                    return "Cast(";
                case "ToLower":
                    return "Lower(";
                case "ToUpper":
                    return "Upper(";
                case "Substring":
                    return "Substring(";
                case "StartsWith":
                    return " like ";
                case "EndsWith":
                case "Contains":
                    return " like '%' + ";
                
                case "Date":
                    return "cast" + BeginWrap();
                case "Hour":
                    return "Datepart('h', ";
                case "Minute":
                    return "Datepart('m', ";
                case "Second":
                    return "Datepart('s', ";
                case "Millisecond":
                    return "Datepart('ms', ";
                case "DayOfWeek":
                    return "Datepart('dw', ";
                case "DayOfYear":
                    return "Datepart('dy', ";
                case "Year":
                case "Month":
                case "Day":
                    return methodCalled + BeginWrap();

            }

            if (_functionTranslation.SupportsMethod(methodCalled))
            {
                return _functionTranslation.TranslateBegin(methodCalled);
            }

            throw new TranslationException(Errors.Translation_MethodCall_NotSupported, new { MethodName = methodCalled });
        }

        public string EndMethodCall(string methodCalled)
        {
            switch ( methodCalled )
            {
                case "ToString":
                    return " as nvarchar(max)" + EndWrap();
                case "ToByte":
                    return " as tinyint" + EndWrap();
                case "ToInt16":
                case "ToSByte":
                    return " as smallint" + EndWrap();
                case "ToInt32":
                case "ToUInt16":
                    return " as int" + EndWrap();
                case "ToInt64":
                case "ToUInt32":
                    return " as bigint" + EndWrap();
                case "ToUInt64":
                    return " as Decimal(20,0)" + EndWrap();
                case "ToDouble":
                    return " as float" + EndWrap();
                case "ToDecimal":
                    return " as decimal(26,3)" + EndWrap();
                case "ToBoolean":
                    return " as bit" + EndWrap();
                case "ToSingle":
                    return " as real" + EndWrap();
                case "ToDateTime":
                    return " as dateTime" + EndWrap();
                case "StartsWith":
                case "Contains":
                    return " + '%'";
                case "EndsWith":
                    return String.Empty;
                case "Date":
                    return " as Date" + EndWrap();
                case "Year":
                case "Day":
                case "Hour":
                case "Minute":
                case "Month":
                case "Second":
                case "DayOfWeek":
                case "DayOfYear":
                case "Millisecond":
                case "ToUpper":
                case "ToLower":
                case "Substring":
                    return EndWrap();
            }

            if (_functionTranslation.SupportsMethod(methodCalled))
            {
                return _functionTranslation.TranslateEnd(methodCalled);
            }

            throw new TranslationException(Errors.Translation_MethodCall_NotSupported, new { MethodName = methodCalled });
        }

        public string FormatConstant(object parseValue)
        {
            if ( parseValue is Double )
            {
                return String.Format(CultureInfo.InvariantCulture, "{0: 0.0#}", parseValue);
            }

            return parseValue.ToString();
        }

        public virtual string FormatTableName(string entityName)
        {
            return Wrap(entityName);
        }

        public string FormatMonthOf(string columnName, int index)
        {
            return FormatFunctionCall("Month", columnName, index);
        }

        public string FormatYearOf(string columnName, int index)
        {
            return FormatFunctionCall("Year", columnName, index);
        }

    }
}
