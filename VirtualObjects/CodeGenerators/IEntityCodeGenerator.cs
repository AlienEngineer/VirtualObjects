using System;
using System.Data;
using VirtualObjects.Queries.Mapping;

namespace VirtualObjects.CodeGenerators
{
    interface IEntityCodeGenerator
    {
        
        void GenerateCode();
        void PrintCode();

        Func<object, IDataReader, object[], MapResult> GetEntityMapper();
        Func<Object> GetEntityProvider();        
        Func<ISession, Object> GetEntityProxyProvider();
        Func<Object, Object> GetEntityCast();
        Func<Int32> GetEntityFieldCount();
    }
}