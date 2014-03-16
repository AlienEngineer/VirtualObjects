using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualObjects.CodeGenerators
{
    class DynamicModelCodeGenerator : EntityCodeGenerator
    {
        readonly TypeBuilder builder;

        public DynamicModelCodeGenerator(Type type)
            : base("Internal_Builder_Dynamic_" + type.Name)
        {
            builder = new TypeBuilder("Internal_Builder_Dynamic_" + type.Name);
        }

        protected override string GenerateMapObjectCode()
        {
            throw new NotImplementedException();
        }

        protected override string GenerateMakeCode()
        {
            throw new NotImplementedException();
        }

        protected override string GenerateMakeProxyCode()
        {
            throw new NotImplementedException();
        }

        protected override string GenerateOtherMethodsCode()
        {
            throw new NotImplementedException();
        }
    }
}
