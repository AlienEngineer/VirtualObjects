using System;

namespace VirtualObjects.Queries.Formatters
{
    public interface IFormatter
    {
        String FormatFieldWithTable(String name, int index);

        String FormatTableName(String name, int index);

        String FieldSeparator { get; }

        String FormatField(String name);
    }
}