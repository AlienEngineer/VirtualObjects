using System;
using FluentAssertions;
using NUnit.Framework;
using VirtualObjects.CodeGenerators;
using System.Collections.Generic;
using System.Dynamic;

namespace VirtualObjects.Tests.CodeGenerators
{

    [TestFixture, Category("Code Generators")]
    public class CodeGeneratorTests
    {
        private readonly IEntityCodeGenerator dynCodeGen;

        public CodeGeneratorTests()
        {
            dynCodeGen = GetDynamicModelCodeGenerator();
        }

        private static IEntityCodeGenerator GetDynamicModelCodeGenerator()
        {
            var obj = new
            {
                Id = 1,
                Name = "This is a test"
            };

            IEntityCodeGenerator dynCodeGen = new DynamicModelCodeGenerator(obj.GetType());

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

            dynamic mapped = Map(new object[] { 7, "Sérgio" }, make(null));

            ((String)mapped.Name).Should().Be("Sérgio");

            ((int)mapped.Id).Should().Be(7);
        }

        public T Map<T>(Object[] data, T obj)
        {
            var map = dynCodeGen.GetEntityMapper();

            return (T)map(obj, data);
        }

    }
}
