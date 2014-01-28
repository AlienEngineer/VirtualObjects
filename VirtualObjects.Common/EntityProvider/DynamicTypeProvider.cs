using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fasterflect;

namespace VirtualObjects.EntityProvider
{
    public class DynamicTypeProvider : IEntityProvider
    {
        public IEntityProvider MainProvider { get; set; }

        public object CreateEntity(Type type)
        {
            ICollection<Object> args = new List<Object>();
            foreach ( var field in type.GetConstructors().First().GetParameters() )
            {
                Object arg = null;

                if ( !field.ParameterType.IsFrameworkType() || (field.ParameterType.InheritsOrImplements<IEnumerable>() && field.ParameterType != typeof(string)) )
                {
                    arg = MainProvider.CreateEntity(field.ParameterType);
                }

                args.Add(arg);
            }

            return Activator.CreateInstance(type, args.ToArray());
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