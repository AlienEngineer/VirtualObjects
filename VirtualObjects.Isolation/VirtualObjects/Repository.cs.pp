﻿using System;
using System.Linq;
using VirtualObjects;

namespace $rootnamespace$.VirtualObjects
{
    class Repository : IRepository, IDisposable
    {
        internal ISession Session;

        public Repository()
            : this(new Session(new Configuration(), connectionName: null))
        {

        }

        public Repository(ISession session)
        {
            Session = session;
        }

        public Repository(String connectionName)
            : this(new Session(new Configuration(), connectionName))
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

		
        #region IRepository Helpers

        public  TResult WithinTransaction<TResult>(Func<TResult> execute)
        {
            return Session.WithinTransaction(execute);
        }

        public  void WithinTransaction( Action execute)
        {
            Session.WithinTransaction(execute);
        }

        /// <summary>
        /// Keeps the session alive. Doesn't close the connection to the database after each operation is complete.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="execute">The execute.</param>
        /// <returns></returns>
        public TResult KeepAlive<TResult>(Func<TResult> execute)
        {
            return Session.KeepAlive(execute);
        }

        /// <summary>
        /// Keeps the session alive. Doesn't close the database connection after each operation is complete.
        /// </summary>
        /// <param name="execute">The execute.</param>
        public void KeepAlive(Action execute)
        {
            Session.KeepAlive(execute);
        }

        public IQueryable<TEntity> Query<TEntity>() where TEntity : class, new()
        {
            return GetAll<TEntity>();
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

}