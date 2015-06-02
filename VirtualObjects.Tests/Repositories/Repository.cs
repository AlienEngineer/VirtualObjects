using System;
using System.Linq;
using VirtualObjects.Config;
using VirtualObjects.Connections;
using VirtualObjects.Mappings;

namespace VirtualObjects.Tests.Repositories
{
    class Repository : IRepository, IDisposable
    {
        internal ISession Session;

        public Repository() : this(new Session(new Configuration()))
        {

        }

        public Repository(ISession session)
        {
            Session = session;
        }

        public Repository(string connectionName)
            : this(new Session(new Configuration
            {
                ConnectionProvider = new NamedDbConnectionProvider(connectionName)
            }))
        {

        }
        
        #region IRepository Members

        public IQueryable<TEntity> GetAll<TEntity>() where TEntity : class, new()
        {
            return ExceptionWrapper.Wrap(() => Session.GetAll<TEntity>());
        }

        public TEntity GetById<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return ExceptionWrapper.Wrap(() => Session.GetById(entity));
        }

        public TEntity Insert<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return ExceptionWrapper.Wrap(() => Session.Insert(entity));
        }

        public TEntity Update<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return ExceptionWrapper.Wrap(() => Session.Update(entity));
        }

        public bool Delete<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return ExceptionWrapper.Wrap(() => Session.Delete(entity));
        }

        public IRepositoryTransaction BeginTransaction()
        {
            return new RepositoryTransaction(Session.BeginTransaction());
        }

        public IRepository CreateNewRepository(string connectionName)
        {
            return new Repository(connectionName);
        }

        #endregion

		#region IDisposable Members
        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if ( !_disposed )
            {
                if ( disposing )
                {
                    Session.Dispose();
                }

                Session = null;

                _disposed = true;
            }
        }

        #endregion

		
    }

    class RepositoryTransaction : IRepositoryTransaction, IDisposable
    {
        private ITransaction _transaction;

        public RepositoryTransaction(ITransaction transaction)
        {
            _transaction = transaction;
        }

        #region IRepositoryTransaction Members

        public void Rollback()
        {
            ExceptionWrapper.Wrap(() => _transaction.Rollback());
        }

        public void Commit()
        {
            ExceptionWrapper.Wrap(() => _transaction.Commit());
        }

        #endregion

		#region IDisposable Members
        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if ( !_disposed )
            {
                if ( disposing )
                {
                    _transaction.Dispose();
                }

                _transaction = null;

                _disposed = true;
            }
        }

        #endregion
    }

    static class ExceptionWrapper
    {
        public static TResult Wrap<TResult>(Func<TResult> execute)
        {
            try
            {
                return execute();
            }
            catch (Exception ex)
            {
                throw new VirtualObjectsException(ex);
            }
        }

        public static void Wrap(Action execute)
        {
            try
            {
                execute();
            }
            catch ( Exception ex )
            {
                throw new VirtualObjectsException(ex);
            }
        }
    }

	public class VirtualObjectsException : Exception
    {
        public VirtualObjectsException(Exception innerException): base(innerException.Message, innerException) { }
		public VirtualObjectsException(string message): base(message) { }
    }

	class Configuration : SessionConfiguration
    {
        
        public override void Initialize()
        {
			// base.Initialize();

            //
            // Use this to configure multiple data connection on app.config
            // It will use the machine name to select the proper connection string.
            //
            //ConnectionProvider = new NamedDbConnectionProvider();
            
            //
            // Use this if you want to use the first connection string configured on app.config
            //
            ConnectionProvider = ConnectionProvider ?? new FirstConnectionDbConnectionProvider();
            
            //
            // Use this if you want to use a specific non configurable connection string.
            //
            // ConnectionProvider = new DbConnectionProvider("System.Data.SqlClient", @"
            //          Data Source=(LocalDB)\v11.0;
            //          AttachDbFilename=|DataDirectory|\database.mdf;
            //          Integrated Security=True;
            //          Connect Timeout=30");

#if DEBUG
            // Use this text writer to print out the commands that are generated and executed.
            // usefull for debug purposes only.
            Logger = Logger ?? Console.Out;

			//
			// Outputs generated code to files.
			// SaveGeneratedCode = true;
#endif
        }


        /// <summary>
        /// Configures the mapping builder. Override this method to define the rules how entities are mapped.
        /// Use this to configure custom Attributes or custom conventions.
        /// </summary>
        /// <param name="builder">The builder.</param>
        public override void ConfigureMappingBuilder(ITranslationConfigurationBuilder builder)
        {
			//
			// Keep Virtual Objects base configurations
			base.ConfigureMappingBuilder(builder);
			
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