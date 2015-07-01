namespace VirtualObjects
{
    /// <summary>
    /// Definition of performance diagnostic options. Enable or disable flags to see times printed on console.
    /// </summary>
    public class PerformanceDiagnosticOptions
    {

        /// <summary>
        /// Gets or sets a value indicating whether [data fetch].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [data fetch]; otherwise, <c>false</c>.
        /// </value>
        public bool DataFetch { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether [field mapping].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [field mapping]; otherwise, <c>false</c>.
        /// </value>
        public bool FieldMapping { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [entity mapping].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [entity mapping]; otherwise, <c>false</c>.
        /// </value>
        public bool EntityMapping { get; set; }

    }
}
