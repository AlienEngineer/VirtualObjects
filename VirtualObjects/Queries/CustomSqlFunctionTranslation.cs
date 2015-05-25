using System.Linq;
using VirtualObjects.Queries.Formatters;
using VirtualObjects.Queries.Translation;

namespace VirtualObjects.Queries
{
    class CustomSqlFunctionTranslation : ICustomFunctionTranslation
    {
        private readonly string[] _methodNames;

        public CustomSqlFunctionTranslation(params string[] methodNames)
        {
            _methodNames = methodNames;
        }

        public bool SupportsMethod(string methodName)
        {
            return _methodNames.Contains(methodName);
        }

        public StringBuffer TranslateBegin(string methodName)
        {
            return "dbo." + methodName + "(";
        }

        public StringBuffer TranslateEnd(string methodName)
        {
            return ")";
        }
    }
}
