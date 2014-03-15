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

        public void GenerateCode()
        {
            throw new NotImplementedException();
        }

        public Action<Object, Object[]> GetEntityMapper()
        {
            return (Action<Object, Object[]>)builder.GetDelegate<Action<Object, Object[]>>("MapObject");
        }

        public Func<Object> GetEntityProvider()
        {
            return (Func<Object>)builder.GetDelegate<Func<Object>>("Make");
        }

        public Func<ISession, Object> GetEntityProxyProvider()
        {
            return (Func<ISession, Object>)builder.GetDelegate<Func<ISession, Object>>("MakeProxy");
        }
    }
}
