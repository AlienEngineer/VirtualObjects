using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualObjects
{
    public static class ReflectionExtensions
    {

        public static Boolean IsDynamic(this Type type)
        {
            return type.Name.StartsWith("<>");
        }

    }
}
