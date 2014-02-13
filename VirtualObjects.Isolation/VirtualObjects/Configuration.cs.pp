using System;
using VirtualObjects;
using VirtualObjects.Config;
using VirtualObjects.Connections;

namespace $rootnamespace$.VirtualObjects
{
    class Configuration : SessionConfiguration
    {
        
        public override void Initialize()
        {
            //
            // Use this to configure multiple data connection on app.config
            // It will use the machine name to select the proper connection string.
            //
            ConnectionProvider = ConnectionProvider ?? new NamedDbConnectionProvider();
            
            //
            // Use this if you want to use the first connection string configured on app.config
            //
            // ConnectionProvider = ConnectionProvider ?? new FirstConnectionDbConnectionProvider();
            
            //
            // Use this if you want to use a specific non configurable connection string.
            //
            // ConnectionProvider = ConnectionProvider ?? new DbConnectionProvider("System.Data.SqlClient", @"
            //          Data Source=(LocalDB)\v11.0;
            //          AttachDbFilename=|DataDirectory|\database.mdf;
            //          Integrated Security=True;
            //          Connect Timeout=30");

#if DEBUG
            // Use this text writer to print out the commands that are generated and executed.
            // usefull for debug purposes only.
            Logger = Console.Out;
#endif
        }


        /// <summary>
        /// Configures the mapping builder. Override this method to define the rules how entities are mapped.
        /// Use this to configure custom Attributes or custom conventions.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public override void ConfigureMappingBuilder(IMappingBuilder builder)
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

            // (e.g.) A key is a property that ends with 'Id'
            // builder.ColumnKeyFromProperty(e => e.Name.EndsWith("Id"));
            //
            builder.ColumnKeyFromAttribute<KeyAttribute>();
            builder.ColumnKeyFromAttribute<IdentityAttribute>();

            builder.ColumnIdentityFromAttribute<IdentityAttribute>();

            builder.ForeignKeyFromAttribute<AssociationAttribute>(e => e.OtherKey);

            builder.ColumnVersionFromProperty(e => e.Name == "Version");
            builder.ColumnVersionFromAttribute<VersionAttribute>();
        }

    }
}
