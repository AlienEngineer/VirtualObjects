using System.Linq;
using VirtualObjects.Queries.Translation;

namespace VirtualObjects.Queries
{
    /// <summary>
    /// Trnslates a custom function invokation into TSQL.
    /// </summary>
    /// <seealso cref="VirtualObjects.Queries.Translation.ICustomFunctionTranslation" />
    public class CustomSqlFunctionTranslation : ICustomFunctionTranslation
    {
        private readonly string[] _methodNames;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomSqlFunctionTranslation"/> class.
        /// </summary>
        /// <param name="methodNames">The method names.</param>
        public CustomSqlFunctionTranslation(params string[] methodNames)
        {
            _methodNames = methodNames;
        }

        /// <summary>
        /// Determines if a method is supported by the current instance of <see cref="ICustomFunctionTranslation" />
        /// </summary>
        /// <param name="methodName">The name of the called method.</param>
        /// <returns>
        ///   <c>true</c> if supported, otherwise <c>false</c>
        /// </returns>
        public bool SupportsMethod(string methodName)
        {
            return _methodNames.Contains(methodName);
        }

        /// <summary>
        /// Translates the beginning of the method.
        /// <code>
        /// MethodName(...
        /// </code>
        /// </summary>
        /// <param name="methodName">The name of the called method.</param>
        /// <returns>
        /// e.g. MethodName(
        /// </returns>
        public StringBuffer TranslateBegin(string methodName)
        {
            return "dbo." + methodName + "(";
        }

        /// <summary>
        /// Translated the end of the method call.
        /// <code>
        /// ...)
        /// </code>
        /// </summary>
        /// <param name="methodName">The name of the called method.</param>
        /// <returns>
        /// e.g. )
        /// </returns>
        public StringBuffer TranslateEnd(string methodName)
        {
            return ")";
        }
    }
}
