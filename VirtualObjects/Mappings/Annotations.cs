using System;

namespace VirtualObjects.Mappings
{

    /// <summary>
    /// 
    /// </summary>
    public class ColumnAttribute : Attribute
    {

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        public String FieldName { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class KeyAttribute : ColumnAttribute
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public class IdentityAttribute : KeyAttribute
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public class TableAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>
        /// The name of the table.
        /// </value>
        public String TableName { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class AssociationAttribute : ColumnAttribute
    {
        /// <summary>
        /// Gets or sets the other key.
        /// </summary>
        /// <value>
        /// The other key.
        /// </value>
        public string OtherKey { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class VersionAttribute : ColumnAttribute
    {

    }


}