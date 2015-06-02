using System;
using System.Data;
using VirtualObjects.Queries.Mapping;

namespace VirtualObjects.CodeGenerators
{
    interface IEntityCodeGenerator
    {
        
        void GenerateCode();
        void PrintCode();

        Func<object, IDataReader, MapResult> GetEntityMapper();
        Func<object> GetEntityProvider();        
        Func<ISession, object> GetEntityProxyProvider();
        Func<object, object> GetEntityCast();
    }
}