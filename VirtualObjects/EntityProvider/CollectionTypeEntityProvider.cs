using System;
using System.Collections;
using System.Collections.Generic;
using VirtualObjects.Reflection;

namespace VirtualObjects.EntityProvider
{
    class CollectionTypeEntityProvider : EntityModelProvider
    {
        public override bool CanCreate(Type type)
        {
            return type.InheritsOrImplements<IEnumerable>();
        }

        public override object CreateEntity(Type type)
        {
            var typeToCreate = typeof(List<>).MakeGenericType(type.GetGenericArguments()[0]);
            return Activator.CreateInstance(typeToCreate);
        }
    }
}