using System;
using System.Linq;

namespace VirtualObjects.Queries.Formatters
{
    class SqlFormatter : IFormatter
    {
        private const string Separator = ", ";

        public String FieldSeparator
        {
            get { return Separator; }
        }

        public String FormatField(String name)
        {
            return "[" + name + "]";
        }

        public String FormatFieldWithTable(String name, int index)
        {
            return FormatField("T" + index) + "." + FormatField(name);
        }

        public String FormatTableName(String name, int index)
        {
            return FormatField(name) + " " + FormatField("T" + index);
        }
    }
}
