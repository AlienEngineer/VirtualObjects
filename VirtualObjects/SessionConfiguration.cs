using System.IO;
using VirtualObjects.Config;
using VirtualObjects.Connections;

namespace VirtualObjects
{
    public class SessionConfiguration
    {
        internal IMappingBuilder MappingBuilder
        {
            get
            {
                return InternalConfigureMappingBuilder(_container.Get<IMappingBuilder>());
            }
        }

        public IDbConnectionProvider ConnectionProvider { get; set; }
        public TextWriter Logger { get; set; }

        private IOcContainer _container;

        internal void Init(IOcContainer container)
        {
            _container = container;
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