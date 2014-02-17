using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using VirtualObjects.Connections;
using VirtualObjects.Exceptions;
using VirtualObjects.Queries.Formatters;

namespace VirtualObjects
{

    /// <summary>
    /// 
    /// </summary>
    public class Session : ISession
    {

        internal ISession InternalSession { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        public Session()
            : this(configuration: null, connectionName: null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="connectionProvider">The connection provider.</param>
        public Session(SessionConfiguration configuration = null, IDbConnectionProvider connectionProvider = null)
            : this(new NinjectContainer(configuration, connectionProvider)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="connectionName">Name of the connection.</param>
        public Session(SessionConfiguration configuration = null, String connectionName = null)
            : this(new NinjectContainer(configuration, connectionName)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Session(SessionConfiguration configuration)
            : this(new NinjectContainer(configuration)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public Session(IOcContainer container)
        {
            InternalSession = container.Get<ISession>();
        }

        /// <summary>
        /// Gets all entities of TEntity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns></returns>
        public IQueryable<TEntity> GetAll<TEntity>() where TEntity : class, new()
        {
            return InternalSession.GetAll<TEntity>();
        }

        /// <summary>
        /// Gets the entity by its ID.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public TEntity GetById<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return InternalSession.GetById(entity);
        }

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public TEntity Insert<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return InternalSession.Insert(entity);
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public TEntity Update<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return InternalSession.Update(entity);
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public bool Delete<TEntity>(TEntity entity) where TEntity : class, new()
        {
            return InternalSession.Delete(entity);
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <returns></returns>
        public ITransaction BeginTransaction()
        {
            return InternalSession.BeginTransaction();
        }

        #region IDisposable Members
        private bool _disposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if ( !_disposed )
            {
                if ( disposing )
                {
                    InternalSession.Dispose();
                }

                InternalSession = null;
                _disposed = true;
            }
        }

        #endregion

    }


    /// <summary>
    /// A session that will connect to an excel file.
    /// Default Provider: System.Data.OleDb
    /// ConnectionString : Provider=Microsoft.ACE.OLEDB.12.0;Data Source='{0}';Extended Properties='Excel 12.0;HDR=YES;IMEX=1;';
    /// </summary>
    public class ExcelSession : Session
    {
        /// <summary>
        /// 
        /// </summary>
        public enum Extension
        {
            /// <summary>
            /// The XLS
            /// </summary>
            Xls,
            /// <summary>
            /// The XLSX
            /// </summary>
            Xlsx,
        }

        static readonly IDictionary<String, String> Masks = new Dictionary<String, String>();

        static ExcelSession()
        {
            Masks.Add(Extension.Xls.ToString().ToLower(), "Provider=Microsoft.Jet.OLEDB.4.0;Data Source='{0}';Extended Properties='Excel 8.0;HDR=YES;';");
            Masks.Add(Extension.Xlsx.ToString().ToLower(), "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='{0}';Extended Properties='Excel 12.0;HDR=YES;'");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelSession" /> class.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="configuration"></param>
        public ExcelSession(string filename, SessionConfiguration configuration = null)
            : base(PrepareConfiguration(configuration), new DbConnectionProvider("System.Data.OleDb", BuildConnectionString(filename.ToLower())))
        { }

        private static SessionConfiguration PrepareConfiguration(SessionConfiguration configuration)
        {
            if (configuration == null)
            {
                configuration = new SessionConfiguration();
            }

            configuration.Formatter = new ExcelFormatter();

            return configuration;
        }

        private static String BuildConnectionString(String filename)
        {
            try
            {
                return String.Format(Masks[ParseExtension(filename)], filename);
            }
            catch ( Exception ex )
            {
                throw new VirtualObjectsException(Errors.Excel_UnableToCreateConnectionString, new { FileName = filename }, ex);
            }
        }

        private static String ParseExtension(String filename)
        {
            return filename.Substring(filename.LastIndexOf('.') + 1);
        }

    }
}