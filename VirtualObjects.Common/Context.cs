﻿using System;
using System.Collections.Generic;
using System.Data;

namespace VirtualObjects
{
    
    public class Context
    {
        public IConnection Connection { get; set; }
    }

    public interface IConnection
    {
        object ExecuteScalar(string commandText, IDictionary<string, IOperationParameter> parameters);
        IDataReader ExecuteReader(string commandText, IDictionary<string, IOperationParameter> parameters);
        void ExecuteNonQuery(string commandText, IDictionary<string, IOperationParameter> parameters);

        ITranslation BeginTranslation();
        IDbConnection DbConnection { get; }
        void Close();
    }

    public interface ITranslation
    {
        IDbConnection DbConnection { get; }
        void Rollback();
        void Commit();
    }
}
