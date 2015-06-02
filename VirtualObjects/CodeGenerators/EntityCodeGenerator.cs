using System;
using System.Collections.Generic;
using System.Data;
using VirtualObjects.Queries.Mapping;

namespace VirtualObjects.CodeGenerators
{
    abstract class EntityCodeGenerator : IEntityCodeGenerator
    {
        readonly TypeBuilder builder;

        private readonly IList<Type> referencedtypes;

        protected EntityCodeGenerator(string typeName, Type baseType, SessionConfiguration configuration, bool IsDynamic = false)
        {
            builder = new TypeBuilder(typeName, baseType, configuration)
            {
                IsDynamic = IsDynamic
            };

            TypeName = typeName;
            referencedtypes = new List<Type>();
        }

        public string TypeName { get; private set; }

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

        /// <summary>
        /// Adds the assembly of the given type to the new assembly as dependency.
        /// </summary>
        /// <param name="type">The type.</param>
        protected void AddReference(Type type)
        {
            if (type == null || type == typeof(object) || referencedtypes.Contains(type))
            {
                return;
            }

            referencedtypes.Add(type);

            builder.References.Add(type.Assembly.Location);

            foreach (var argType in type.GetGenericArguments())
            {
                AddReference(argType);
            }

            foreach (var prop in type.GetProperties())
            {
                AddReference(prop.PropertyType);
            }

            foreach (var field in type.GetFields())
            {
                AddReference(field.FieldType);
            }

            AddReference(type.BaseType);
        }

        /// <summary>
        /// Adds the namespace.
        /// </summary>
        /// <param name="nameSpace">The name space.</param>
        protected void AddNamespace(string nameSpace)
        {
            builder.Namespaces.Add(nameSpace);
        }

        protected abstract string GenerateMapObjectCode();
        protected abstract string GenerateMakeCode();
        protected abstract string GenerateMakeProxyCode();
        protected abstract string GenerateOtherMethodsCode();

                           
        public void PrintCode()
        {

            Console.WriteLine("-----------------------------------------------------------"); 
            Console.WriteLine(" -> Code generated for : {0} <-", builder.TypeName);
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine(builder.Code);
            Console.WriteLine("-----------------------------------------------------------");
        }

        public Func<object, IDataReader, MapResult> GetEntityMapper()
        {
            return (Func<object, IDataReader, MapResult>)builder.GetDelegate<Func<object, IDataReader, MapResult>>("MapObject");
        }

        public Func<object> GetEntityProvider()
        {
            return (Func<object>)builder.GetDelegate<Func<object>>("Make");
        }

        public Func<ISession, object> GetEntityProxyProvider()
        {
            return (Func<ISession, object>)builder.GetDelegate<Func<ISession, object>>("MakeProxy");
        }

        public Func<object, object> GetEntityCast()
        {
            return (Func<object, object>)builder.GetDelegate<Func<object, object>>("EntityCast");
        }

        public Action<Type> GetInitializer()
        {
            return (Action<Type>)builder.GetDelegate<Action<Type>>("Init");
        }
    }
}