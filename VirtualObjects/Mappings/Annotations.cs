using System;

namespace VirtualObjects.Mappings
{

    /// <summary>
    /// Use this attribute to bind this property to a data source field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
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
    /// Indicates that this property is a key.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class KeyAttribute : ColumnAttribute
    {

    }

    /// <summary>
    /// Indicates that this property is a identity field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IdentityAttribute : KeyAttribute
    {

    }

    /// <summary>
    /// Bind this class to a data source table.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
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
    /// Binds this property to a foreignKey.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AssociationAttribute : ColumnAttribute
    {
        /// <summary>
        /// Gets or sets the other keys.
        /// <para>Use ; to separate fields.</para>
        /// <para>A bind should look like [Property1]:[Property2]</para>
        /// </summary>
        /// <value>
        /// The other keys.
        /// </value>
        public string Bind { get; set; }

        /// <summary>
        /// Gets or sets the other key.
        /// </summary>
        /// <value>
        /// The other key.
        /// </value>
        public string OtherKey { get; set; }
    }

    /// <summary>
    /// Use this property for Version Control
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class VersionAttribute : ColumnAttribute
    {

    }

    /// <summary>      
    /// Indicates that this property should be ignored.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IgnoreAttribute : ColumnAttribute
    {

    }

    /// <summary>      
    /// Indicates that this property should be ignored for changes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ComputedAttribute : ColumnAttribute
    {

    }

    /// <summary>
    /// Filters a Collection field with the field specified in this attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FilterWith : Attribute
    {

        /// <summary>
        /// Gets or sets the name of the field you wish to filter the collection with.
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        public String FieldName { get; set; }

    }
}