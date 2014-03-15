using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualObjects.CodeGenerators
{
    interface IEntityCodeGenerator
    {
        void GenerateCode();
        Action<Object, Object[]> GetEntityMapper();
        Func<Object> GetEntityProvider();
        Func<ISession, Object> GetEntityProxyProvider();
    }

    abstract class EntityCodeGenerator : IEntityCodeGenerator
    {
        readonly TypeBuilder builder;

        public EntityCodeGenerator(String typeName)
        {
            builder = new TypeBuilder(typeName);
        }

        public void GenerateCode()
        {

            var mapObject = GenerateMapObjectCode();

        }

        protected abstract String GenerateMapObjectCode();

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