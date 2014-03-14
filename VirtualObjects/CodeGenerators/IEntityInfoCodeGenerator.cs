using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualObjects.CodeGenerators
{
    interface IEntityInfoCodeGenerator
    {
        void GenerateCode();
        Action<Object, Object[]> GetEntityMapper();
        Func<Object> GetEntityProvider();
        Func<ISession, Object> GetEntityProxyProvider();
    }
}