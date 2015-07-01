using System;
using System.Collections.Generic;
using System.Data;
using VirtualObjects.Queries.Mapping;

namespace VirtualObjects.CodeGenerators
{
    abstract class EntityCodeGenerator : IEntityCodeGenerator
    {
        readonly TypeBuilder _builder;

        private readonly IList<Type> _referencedtypes;

        protected EntityCodeGenerator(string typeName, Type baseType, SessionConfiguration configuration, bool IsDynamic = false)
        {
            _builder = new TypeBuilder(typeName, baseType, configuration)
            {
                IsDynamic = IsDynamic
            };

            TypeName = typeName;
            Configuration = configuration;
            _referencedtypes = new List<Type>();
        }

        public string TypeName { get; private set; }
        public SessionConfiguration Configuration { get; set; }

        public void GenerateCode()
        {
            var mapObject = GenerateMapObjectCode();
            var make = GenerateMakeCode();
            var makeProxy = GenerateMakeProxyCode();
            var otherMethods = GenerateOtherMethodsCode();

            _builder.Body.Add(mapObject);
            _builder.Body.Add(make);
            _builder.Body.Add(makeProxy);
            _builder.Body.Add(otherMethods);
        }

        /// <summary>
        /// Adds the assembly of the given type to the new assembly as dependency.
        /// </summary>
        /// <param name="type">The type.</param>
        protected void AddReference(Type type)
        {
            if (type == null || type == typeof(object) || _referencedtypes.Contains(type))
            {
                return;
            }

            _referencedtypes.Add(type);

            _builder.References.Add(type.Assembly.Location);

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
            _builder.Namespaces.Add(nameSpace);
        }

        protected abstract string GenerateMapObjectCode();
        protected abstract string GenerateMakeCode();
        protected abstract string GenerateMakeProxyCode();
        protected abstract string GenerateOtherMethodsCode();

                           
        public void PrintCode()
        {

            Console.WriteLine("-----------------------------------------------------------"); 
            Console.WriteLine(" -> Code generated for : {0} <-", _builder.TypeName);
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine(_builder.Code);
            Console.WriteLine("-----------------------------------------------------------");
        }

        public Func<object, IDataReader, MapResult> GetEntityMapper()
        {
            return (Func<object, IDataReader, MapResult>)_builder.GetDelegate<Func<object, IDataReader, MapResult>>("MapObject");
        }

        public Func<object> GetEntityProvider()
        {
            return (Func<object>)_builder.GetDelegate<Func<object>>("Make");
        }

        public Func<ISession, object> GetEntityProxyProvider()
        {
            return (Func<ISession, object>)_builder.GetDelegate<Func<ISession, object>>("MakeProxy");
        }

        public Func<object, object> GetEntityCast()
        {
            return (Func<object, object>)_builder.GetDelegate<Func<object, object>>("EntityCast");
        }

        public Action<Type> GetInitializer()
        {
            return (Action<Type>)_builder.GetDelegate<Action<Type>>("Init");
        }
    }
}