using System;
using System.Collections.Generic;
using System.Data;

namespace VirtualObjects
{
    public interface IConnection : IDisposable
    {
        object ExecuteScalar(string commandText, IDictionary<string, IOperationParameter> parameters);
        IDataReader ExecuteReader(string commandText, IDictionary<string, IOperationParameter> parameters);
        void ExecuteNonQuery(string commandText, IDictionary<string, IOperationParameter> parameters);

        ITransaction BeginTranslation();
        IDbConnection DbConnection { get; }
        bool KeepAlive { get; set; }
        void Close();
    }
}