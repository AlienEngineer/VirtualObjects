using System;
using System.Data;
using VirtualObjects.Queries.Mapping;

namespace VirtualObjects.CodeGenerators
{
    abstract class EntityCodeGenerator : IEntityCodeGenerator
    {
        readonly TypeBuilder builder;

        protected EntityCodeGenerator(string typeName, Type baseType, SessionConfiguration configuration, bool IsDynamic = false)
        {
            builder = new TypeBuilder(typeName, baseType, configuration)
            {
                IsDynamic = IsDynamic
            };

            TypeName = typeName;
        }

        public String TypeName { get; private set; }

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
            if (type == null || type == typeof(Object))
            {
                return;
            }

            builder.References.Add(type.Assembly.CodeBase.Remove(0, "file:///".Length));
            AddReference(type.BaseType);
        }

        protected void AddNamespace(String nameSpace)
        {
            builder.Namespaces.Add(nameSpace);
        }

        protected abstract String GenerateMapObjectCode();
        protected abstract String GenerateMakeCode();
        protected abstract String GenerateMakeProxyCode();
        protected abstract String GenerateOtherMethodsCode();

                           
        public void PrintCode()
        {

            Console.WriteLine("-----------------------------------------------------------"); 
            Console.WriteLine(" -> Code generated for : {0} <-", builder.TypeName);
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine(builder.Code);
            Console.WriteLine("-----------------------------------------------------------");
        }

        public Func<Object, IDataReader, MapResult> GetEntityMapper()
        {
            return (Func<Object, IDataReader, MapResult>)builder.GetDelegate<Func<Object, IDataReader, MapResult>>("MapObject");
        }

        public Func<Object> GetEntityProvider()
        {
            return (Func<Object>)builder.GetDelegate<Func<Object>>("Make");
        }

        public Func<ISession, Object> GetEntityProxyProvider()
        {
            return (Func<ISession, Object>)builder.GetDelegate<Func<ISession, Object>>("MakeProxy");
        }

        public Func<Object, Object> GetEntityCast()
        {
            return (Func<Object, Object>)builder.GetDelegate<Func<Object, Object>>("EntityCast");
        }

        public Action<Type> GetInitializer()
        {
            return (Action<Type>)builder.GetDelegate<Action<Type>>("Init");
        }
    }
}