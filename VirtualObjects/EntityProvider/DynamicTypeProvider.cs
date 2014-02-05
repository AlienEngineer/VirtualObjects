using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fasterflect;

namespace VirtualObjects.EntityProvider
{

    class DynamicTypeProvider : IEntityProvider
    {
        public IEntityProvider MainProvider { get; set; }

        private Type _type;
        private IEnumerable<object> _arguments;

        public void PrepareProvider(Type outputType)
        {
            _type = outputType;
            
            // 
            // Store the arguments with the possibility to create entity of reference type.
            // a reference type is detected by not beeing null.
            //
            _arguments = GetArguments(_type)
                .Select(e => e != null ? MainProvider.CreateEntity(e.GetType()) : e);
        }

        public object CreateEntity(Type type)
        {
            var args = ( type == _type ) ? _arguments : GetArguments(type);

            return Activator.CreateInstance(type, args.ToArray());
        }

        private IEnumerable<object> GetArguments(Type type)
        {
            ICollection<Object> args = new List<Object>();

            foreach (var field in type.GetConstructors().First().GetParameters())
            {
                Object arg = null;

                if (!field.ParameterType.IsFrameworkType() ||
                    (field.ParameterType.InheritsOrImplements<IEnumerable>() && field.ParameterType != typeof (string)))
                {
                    arg = MainProvider.CreateEntity(field.ParameterType);
                }

                args.Add(arg);
            }
            return args;
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