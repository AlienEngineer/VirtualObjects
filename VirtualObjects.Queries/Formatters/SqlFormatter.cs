using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using VirtualObjects.Config;

namespace VirtualObjects.Queries.Formatters
{
    class SqlFormatter : IFormatter
    {
        private const string Separator = ", ";
        private const string TablePrefix = "T";

        public SqlFormatter()
        {
            Select = "Select";
            From = "From";
            Where = "Where";
            And = "And";
            In = "In";
        }

        private static string Wrap(string name)
        {
            return string.Format("[{0}]", name);
        }

        public String FieldSeparator
        {
            get { return Separator; }
        }

        public string Select { get; private set; }
        public string From { get; private set; }
        public string Where { get; private set; }
        public string And { get; private set; }
        public string In { get; private set; }

        public String FormatField(String name)
        {
            return Wrap(name);
        }

        public String FormatFieldWithTable(String name, int index)
        {
            return string.Format("{0}.{1}", GetTableAlias(index), Wrap(name));
        }

        public String FormatTableName(String name, int index)
        {
            return string.Format("{0} {1}", Wrap(name), GetTableAlias(index));
        }

        public string FormatNode(ExpressionType nodeType)
        {
            switch (nodeType)
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
                    throw new UnsupportedException(Errors.SQL_UnableToFormatNode, new {NodeType = nodeType});

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

            return new StringBuilder()
                .Append("ROW_NUMBER() OVER ( Order By ")
                .Append(FormatFields(columns, 100 + index))
                .Append(") as [Internal_Row_Index], *")
                .ToString();
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
            return FormatField(TablePrefix + index);
        }

        public string BeginWrap()
        {
            return "(";
        }

        public string EndWrap(int i = 1)
        {
            return new string(')', i);
        }
    }
}
