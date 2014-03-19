using System;
using System.Data;

namespace VirtualObjects.CodeGenerators
{
    interface IEntityCodeGenerator
    {
        
        void GenerateCode();
        void PrintCode();

        Func<Object, IDataReader, Object> GetEntityMapper();
        Func<Object> GetEntityProvider();        
        Func<ISession, Object> GetEntityProxyProvider();
        Func<Object, Object> GetEntityCast();
    }
}