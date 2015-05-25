namespace VirtualObjects.Queries.Translation
{
    /// <summary>
    /// Handles the way custom function calls are translated.
    /// </summary>
    public interface ICustomFunctionTranslation
    {
        /// <summary>
        /// Determines if a method is supported by the current instance of <see cref="ICustomFunctionTranslation"/>
        /// </summary>
        /// <param name="methodName">The name of the called method.</param>
        /// <returns><c>true</c> if supported, otherwise <c>false</c></returns>
        bool SupportsMethod(string methodName);
        
        /// <summary>
        /// Translates the beginning of the method.
        /// <code>
        ///     MethodName(...
        /// </code>
        /// </summary>
        /// <param name="methodName">The name of the called method.</param>
        /// <returns>e.g. MethodName(</returns>
        StringBuffer TranslateBegin(string methodName);
        
        /// <summary>
        /// Translated the end of the method call.
        /// <code>
        ///     ...)
        /// </code>
        /// </summary>
        /// <param name="methodName">The name of the called method.</param>
        /// <returns>e.g. )</returns>
        StringBuffer TranslateEnd(string methodName);
    }
}