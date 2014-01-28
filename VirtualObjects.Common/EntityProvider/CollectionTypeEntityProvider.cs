using System;
using System.Collections;
using System.Collections.Generic;
using Fasterflect;

namespace VirtualObjects.EntityProvider
{
    class CollectionTypeEntityProvider : EntityProvider
    {
        public override bool CanCreate(Type type)
        {
            return type.InheritsOrImplements<IEnumerable>();
        }

        public override object CreateEntity(Type type)
        {
            var typeToCreate = typeof(List<>).MakeGenericType(type.GetGenericArguments()[0]);
            return base.CreateEntity(typeToCreate);
        }
    }
}