using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq.Mapping;
using System.Diagnostics;
using System.Reflection;

namespace VirtualObjects.Queries.LinqToSql
{
    internal class EntityMetaModel : MetaModel
    {
        private readonly Type _dataContextType;
        private readonly Type _providerType;

        public EntityMetaModel(Type dataContextType, Type providerType)
        {
            _dataContextType = dataContextType;
            _providerType = providerType;
        }

        #region Overrides of MetaModel

        public override MetaTable GetTable(Type rowType)
        {
            return new EntityMetaTable();
        }

        public override MetaFunction GetFunction(MethodInfo method)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<MetaTable> GetTables()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<MetaFunction> GetFunctions()
        {
            throw new NotImplementedException();
        }

        public override MetaType GetMetaType(Type type)
        {
            throw new NotImplementedException();
        }

        public override MappingSource MappingSource
        {
            get { throw new NotImplementedException(); }
        }

        public override Type ContextType
        {
            get
            {
                Trace.WriteLine("ContextType called");
                return _dataContextType;
            }
        }

        public override string DatabaseName
        {
            get { return "Employee"; }
        }

        public override Type ProviderType
        {
            get { return _providerType; }
        }

        #endregion
    }

    internal class EntityMetaTable : MetaTable
    {
        #region Overrides of MetaTable

        public override MetaModel Model
        {
            get
            {
                Trace.WriteLine("Model called");
                throw new NotImplementedException();
            }
        }

        public override string TableName
        {
            get { return "Employee"; }
        }

        public override MetaType RowType
        {
            get
            {
                Trace.WriteLine("RowType called");
                throw new NotImplementedException();
            }
        }

        public override MethodInfo InsertMethod
        {
            get { throw new NotImplementedException(); }
        }

        public override MethodInfo UpdateMethod
        {
            get { throw new NotImplementedException(); }
        }

        public override MethodInfo DeleteMethod
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}