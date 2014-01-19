using System;
using System.Data;
using System.Data.Linq.Mapping;

namespace VirtualObjects.Queries.LinqToSql
{
    internal class LinqToSqlMapper : MappingSource
    {
        private readonly Type _providerType;

        public LinqToSqlMapper(Type providerType)
        {
            _providerType = providerType;
        }

        #region Overrides of MappingSource

        protected override MetaModel CreateModel(Type dataContextType)
        {
            return new EntityMetaModel(dataContextType, _providerType);
        }

        #endregion
    }
}