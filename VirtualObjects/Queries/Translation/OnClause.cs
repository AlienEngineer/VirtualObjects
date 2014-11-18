namespace VirtualObjects.Queries.Translation
{
    /// <summary>
    /// Represents an Join OnClause
    /// </summary>
    public class OnClause
    {
        /// <summary>
        /// Gets or sets the column1.
        /// </summary>
        /// <value>
        /// The column1.
        /// </value>
        public IEntityColumnInfo Column1 { get; set; }
        /// <summary>
        /// Gets or sets the column2.
        /// </summary>
        /// <value>
        /// The column2.
        /// </value>
        public IEntityColumnInfo Column2 { get; set; }
    }
}
