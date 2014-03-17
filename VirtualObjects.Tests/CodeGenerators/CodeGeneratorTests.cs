﻿using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using VirtualObjects.CodeGenerators;
using System.Collections.Generic;
using System.Dynamic;
using VirtualObjects.Config;

namespace VirtualObjects.Tests.CodeGenerators
{

    [TestFixture, Category("Code Generators")]
    public class CodeGeneratorTests : UtilityBelt
    {
        private readonly IEntityCodeGenerator dynCodeGen;

        public CodeGeneratorTests()
        {
            dynCodeGen = GetDynamicModelCodeGenerator();
        }

        private IEntityCodeGenerator GetDynamicModelCodeGenerator()
        {
            var obj = new
            {
                Id = 1,
                Name = "This is a test"
            };

            IEntityCodeGenerator dynCodeGen = new DynamicModelCodeGenerator(obj.GetType(), Make<IEntityBag>());

            dynCodeGen.GenerateCode();

            //// force compilation.
            //dynCodeGen.GetEntityMapper();

            //dynCodeGen.PrintCode();

            return dynCodeGen;
        }

        [Test]
        public void DynamicType_CodeGenerator_Make()
        {
            var make = dynCodeGen.GetEntityProvider();

            var provided = make();

            provided.Should().NotBeNull();
        }


        [Test]
        public void DynamicType_CodeGenerator_MakeProxy()
        {
            var make = dynCodeGen.GetEntityProxyProvider();

            var provided = make(null);

            provided.Should().NotBeNull();
        }



        [Test]
        public void DynamicType_CodeGenerator_Make_And_Map()
        {
            var make = dynCodeGen.GetEntityProxyProvider();
            var map = dynCodeGen.GetEntityMapper();

            dynamic mapped = map(make(null), new object[] { 7, "Sérgio" });

            ((String)mapped.Name).Should().Be("Sérgio");

            ((int)mapped.Id).Should().Be(7);
        }

        [Test]
        public void DynamicType_CodeGenerator_Make_And_Map_Collection()
        {
            var data = new Object[][] {
                new object[] { 1, "Sérgio" },
                new object[] { 2, "Daniel" },
                new object[] { 3, "Bernardo" },
                new object[] { 4, "Ferreira" }
            };

            var result = MapEntities(new { Numero = 0, Nome = "" }, data);

            result.Count.Should().Be(4);
        }

        public List<TEntity> MapEntities<TEntity>(TEntity entity, Object[][] data)
        {
            IEntityCodeGenerator dynCodeGen = new DynamicModelCodeGenerator(typeof(TEntity), Make<IEntityBag>());

            dynCodeGen.GenerateCode();

            var make = dynCodeGen.GetEntityProxyProvider();
            var map = dynCodeGen.GetEntityMapper();

            type = typeof(TEntity);
            ctor = type.GetConstructors().Single();
            parameters = ctor.GetParameters();

            var result = new List<TEntity>();

            for ( int i = 0; i < data.Length; i++ )
            {
                dynamic mapped = map(make(null), data[i]);
                result.Add((TEntity)Convert(mapped));
            }

            return result;
        }

        private static Type type;
        private static System.Reflection.ConstructorInfo ctor;
        private static System.Reflection.ParameterInfo[] parameters;

        public static Object Convert(Object source)
        {
            IDictionary<string, object> dict = (ExpandoObject)source;

            var parameterValues = parameters.Select(p => dict[p.Name]).ToArray();

            return ctor.Invoke(parameterValues);
        }
    }
}
