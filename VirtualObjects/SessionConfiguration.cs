using System;
using System.IO;
using VirtualObjects.Config;
using VirtualObjects.Connections;
using VirtualObjects.Mappings;
using VirtualObjects.Queries.Formatters;

namespace VirtualObjects
{
    /// <summary>
    /// 
    /// Configuration Settings for a session.
    /// 
    /// </summary>
    public class SessionConfiguration
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionConfiguration"/> class.
        /// </summary>
        public SessionConfiguration()
        {
            TranslationConfigurationBuilder = new TranslationConfigurationBuilder();
        }

        internal ITranslationConfigurationBuilder TranslationConfigurationBuilder { get; set; }

        /// <summary>
        /// Gets or sets the connection provider.
        /// </summary>
        /// <value>
        /// The connection provider.
        /// </value>
        public IDbConnectionProvider ConnectionProvider { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public TextWriter Logger { get; set; }

        /// <summary>
        /// Gets or sets the formatter.
        /// </summary>
        /// <value>
        /// The formatter.
        /// </value>
        public IFormatter Formatter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [save generated code].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [save generated code]; otherwise, <c>false</c>.
        /// </value>
        public Boolean SaveGeneratedCode { get; set; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public virtual void Initialize()
        {
            ConnectionProvider = ConnectionProvider ?? new NamedDbConnectionProvider();              
        }

        /// <summary>
        /// Configures the mapping builder. Override this method to define the rules how entities are mapped.
        /// Use this to configure custom Attributes or custom conventions.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public virtual void ConfigureMappingBuilder(ITranslationConfigurationBuilder builder)
        {
            //
            // Table Mapping
            //
            builder.EntityName(e => e.Name);
            builder.EntityName<TableAttribute>(e => e.TableName);

            //
            // Column Mapping
            //
            builder.ColumnName(e => e.Name);
            builder.ColumnName<ColumnAttribute>(e => e.FieldName);

            builder.ColumnKey<KeyAttribute>();
            builder.ColumnKey<IdentityAttribute>();

            builder.ColumnIdentity<IdentityAttribute>();

            builder.ForeignKey<AssociationAttribute>(e => e.OtherKey);
            builder.ForeignKeyLinks<AssociationAttribute>(e => e.Bind);

            builder.ColumnVersion(e => e.Name == "Version" && e.PropertyType == typeof(byte[]));
            builder.ColumnVersion<VersionAttribute>();

            builder.ColumnIgnore(e => e.Name.StartsWith("Ignore"));
            builder.ColumnIgnore<IgnoreAttribute>();

            builder.ComputedColumn<ComputedAttribute>();
            builder.IsForeignKey<ForeignKeyAttribute>();

            //
            // Collections filters.
            //
            builder.CollectionFilter<FilterWith>(e => e.FieldName);
        }

    }
}