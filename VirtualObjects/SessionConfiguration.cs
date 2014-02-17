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
        internal IMappingBuilder MappingBuilder
        {
            get
            {
                return InternalConfigureMappingBuilder(_container.Get<IMappingBuilder>());
            }
        }

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

        private IOcContainer _container;

        internal void Init(IOcContainer container)
        {
            _container = container;
            Initialize();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public virtual void Initialize()
        {
            
        }

        private IMappingBuilder InternalConfigureMappingBuilder(IMappingBuilder builder)
        {
            ConfigureMappingBuilder(builder);

            return builder;
        }

        /// <summary>
        /// Configures the mapping builder. Override this method to define the rules how entities are mapped.
        /// Use this to configure custom Attributes or custom conventions.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public virtual void ConfigureMappingBuilder(IMappingBuilder builder)
        {
            //
            // TableName getters
            //
            builder.EntityNameFromType(e => e.Name);
            builder.EntityNameFromAttribute<TableAttribute>(e => e.TableName);

            //
            // ColumnName getters
            //
            builder.ColumnNameFromProperty(e => e.Name);
            builder.ColumnNameFromAttribute<ColumnAttribute>(e => e.FieldName);

            builder.ColumnKeyFromAttribute<KeyAttribute>();
            builder.ColumnKeyFromAttribute<IdentityAttribute>();

            builder.ColumnIdentityFromAttribute<IdentityAttribute>();

            builder.ForeignKeyFromAttribute<AssociationAttribute>(e => e.OtherKey);

            builder.ColumnVersionFromProperty(e => e.Name == "Version");
            builder.ColumnVersionFromAttribute<VersionAttribute>();

        }

    }
}