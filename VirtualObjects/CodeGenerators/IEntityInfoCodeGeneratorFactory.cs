using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualObjects.CodeGenerators
{
    interface IEntityInfoCodeGeneratorFactory
    {
        IEntityCodeGenerator Make(IEntityInfo info);
    }
}
