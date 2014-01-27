using System;
using System.Linq;
using System.Reflection;
using Fasterflect;

namespace VirtualObjects.EntityProvider
{
    public class DynamicTypeProvider : IEntityProvider
    {
        public object CreateEntity(Type type)
        {
            var ctor = type.Constructors().First();

            return ctor.Invoke(MakeParameters(ctor));
        }

        private object[] MakeParameters(ConstructorInfo ctor)
        {
            return ctor.Parameters().Select(e => e.DefaultValue).ToArray();
        }

        public bool CanCreate(Type type)
        {
            return type.IsDynamic();
        }

        public IEntityProvider GetProviderForType(Type type)
        {
            return CanCreate(type) ? this : null;
        }
    }
}