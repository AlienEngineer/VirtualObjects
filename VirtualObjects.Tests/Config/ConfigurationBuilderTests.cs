using System;
using System.Linq;


namespace VirtualObjects.Tests.Config
{
    using NUnit.Framework;
    using VirtualObjects.Config;
    using FluentAssertions;

    /// <summary>
    /// 
    /// Description
    /// 
    /// Author: Sérgio
    /// </summary>
    [TestFixture]
    public class ConfigurationBuilderTests
    {
        public class TestModel
        {

            public int SomeName { get; set; }

            [Column("NotSoRandom")]
            public int SomeRandomName { get; set; }

        }

        /// <summary>
        /// 
        /// Description
        /// 
        /// </summary>
        [Test]
        public void ColumnName_Should_Be_Found()
        {

            var builder = new MappingBuilder();

            builder.NameFromProperty(e => e.Name);
            builder.NameFromAttribute<ColumnAttribute>(e => e.Name);

            var mapping = builder.Build();
            var entityInfo = mapping.Map(typeof(TestModel));

            var someNameInfo = entityInfo.Columns.First();
            var notSoRandomInfo = entityInfo.Columns.Skip(1).First();


            //
            // Assertions
            //
            entityInfo.Columns.Should().NotBeEmpty();
            CollectionAssert.AllItemsAreNotNull(entityInfo.Columns);

            someNameInfo.ColumnName.Should().Be("SomeName");
            notSoRandomInfo.ColumnName.Should().Be("NotSoRandom");
        }
    }
    
    
}
