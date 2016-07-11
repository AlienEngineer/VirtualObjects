namespace VirtualObjects
{
    /// <summary>
    /// Definition of a query parameter
    /// </summary>
    public interface IQueryParameter
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        object Value { get; set; }
    }
}