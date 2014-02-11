﻿using System;

namespace VirtualObjects.Queries.Formatters
{
    class ExcelFormatter : SqlFormatter
    {
        public override String FormatTableName(String name, int index)
        {
            return string.Format("{0} {1}", Wrap(name + "$"), GetTableAlias(index));
        }

        public override string FormatTableName(string entityName)
        {
            return Wrap(entityName + "$");
        }
    }
}