using System;

namespace VirtualObjects.CodeGenerators
{
    class DynamicModelCodeGenerator : EntityCodeGenerator
    {
        
        public DynamicModelCodeGenerator(Type type)
            : base("Internal_Builder_Dynamic_" + type.Name)
        {

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
