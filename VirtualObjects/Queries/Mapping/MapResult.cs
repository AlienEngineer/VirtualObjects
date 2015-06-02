namespace VirtualObjects.Queries.Mapping
{
    /// <summary>
    /// 
    /// </summary>
    public class MapResult
    {
        /// <summary>
        /// Gets or sets the entity.
        /// </summary>
        /// <value>
        /// The entity.
        /// </value>
        public object Entity { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [has more].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [has more]; otherwise, <c>false</c>.
        /// </value>
        public bool HasMore { get; set; }
    }
}
