using System;
using System.Linq.Expressions;

namespace VirtualObjects.Queries.Formatters
{
    class SqlFormatter : IFormatter
    {
        private const string Separator = ", ";
        private const string TablePrefix = "T";

        private static string Wrap(string name)
        {
            return string.Format("[{0}]", name);
        }

        public String FieldSeparator
        {
            get { return Separator; }
        }

        public String FormatField(String name)
        {
            return Wrap(name);
        }

        public String FormatFieldWithTable(String name, int index)
        {
            return string.Format("{0}.{1}", Wrap(TablePrefix + index), Wrap(name));
        }

        public String FormatTableName(String name, int index)
        {
            return string.Format("{0} {1}", Wrap(name), Wrap(TablePrefix + index));
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
    }
}
