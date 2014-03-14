using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualObjects.CodeGenerators
{
    interface IEntityInfoCodeGeneratorFactory
    {
        IEntityInfoCodeGenerator Make(IEntityInfo info);
    }
}
