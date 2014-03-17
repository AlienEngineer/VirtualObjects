using System;

namespace VirtualObjects.CodeGenerators
{
    interface IEntityCodeGenerator
    {
        void GenerateCode();
        Action<Object, Object[]> GetEntityMapper();
        Func<Object> GetEntityProvider();
        Func<ISession, Object> GetEntityProxyProvider();
    }
}