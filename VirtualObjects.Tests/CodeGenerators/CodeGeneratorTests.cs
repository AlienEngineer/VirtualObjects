using System;
using FluentAssertions;
using NUnit.Framework;
using VirtualObjects.CodeGenerators;

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

        private static DynamicModelCodeGenerator GetDynamicModelCodeGenerator()
        {
            var obj = new
            {
                Id = 1,
                Name = "This is a test"
            };

            var dynCodeGen = new DynamicModelCodeGenerator(obj.GetType());

            dynCodeGen.GenerateCode();
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
            var make = dynCodeGen.GetEntityProvider();
            var map = dynCodeGen.GetEntityMapper();

            dynamic provided = new { Id = int.MinValue, Name = String.Empty }; // make();

            Map(provided, new object[] { 7, "Sérgio" });

            ((String)provided.Name).Should().Be("Sérgio");

            ((int)provided.Id).Should().Be(7);
        }

        public static void Map(dynamic obj, Object[] data)
        {
            obj.Id = (int)data[0];
            obj.Name = (String) data[1];
        }
    }
}
