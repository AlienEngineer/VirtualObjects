using System;

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
    }
}
