using System.Collections.Generic;

namespace VirtualObjects
{
    class QueryCommand
    {
        public string Command { get; set; }
        public IEnumerable<IQueryParameter> Parameters { get; set; }
    }
}