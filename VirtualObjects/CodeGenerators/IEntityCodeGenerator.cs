using System;

namespace VirtualObjects.CodeGenerators
{
    interface IEntityCodeGenerator
    {
        void GenerateCode();
        void PrintCode();

        Func<Object, Object[], Object> GetEntityMapper();
        Func<Object> GetEntityProvider();        
        Func<ISession, Object> GetEntityProxyProvider();        
    }
}