using System;

namespace VirtualObjects.CodeGenerators
{
    abstract class EntityCodeGenerator : IEntityCodeGenerator
    {
        readonly TypeBuilder builder;

        protected EntityCodeGenerator(String typeName)
        {
            builder = new TypeBuilder(typeName);
        }

        public void GenerateCode()
        {
            var mapObject = GenerateMapObjectCode();
            var make = GenerateMakeCode();
            var makeProxy = GenerateMakeProxyCode();
            var otherMethods = GenerateOtherMethodsCode();

            builder.Body.Add(mapObject);
            builder.Body.Add(make);
            builder.Body.Add(makeProxy);
            builder.Body.Add(otherMethods);
        }

        protected void AddReference(Type type)
        {
            builder.References.Add(type.Assembly.CodeBase.Remove(0, "file:///".Length));
        }

        protected void AddNamespace(String nameSpace)
        {
            builder.Namespaces.Add(nameSpace);
        }

        protected abstract String GenerateMapObjectCode();
        protected abstract String GenerateMakeCode();
        protected abstract String GenerateMakeProxyCode();
        protected abstract String GenerateOtherMethodsCode();


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