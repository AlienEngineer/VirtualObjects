using System;
using System.Linq;


namespace VirtualObjects.Tests.Config
{
    using NUnit.Framework;
    using VirtualObjects.Config;
    using FluentAssertions;

    /// <summary>
    /// 
    /// Unit-tests for MappingBuilder and Mapper
    /// 
    /// Author: Sérgio
    /// </summary>
    [TestFixture, Category("Mapping")]
    public class MappingTests
    {
        public class TestModel
        {

            public int SomeName { get; set; }

            [Db.Identity("NotSoRandom")]
            public int SomeRandomName { get; set; }

        }

        IEntityInfo entityInfo;

        [TestFixtureSetUp]
        public void SetUp()
        {
            entityInfo = MapTestModel();
        }

        private IEntityInfo MapTestModel()
        {
            var mapping = CreateBuilder().Build();
            return mapping.Map(typeof(TestModel));
        }
  
        private MappingBuilder CreateBuilder()
        {
            var builder = new MappingBuilder();

            //
            // TableName getters
            //
            builder.EntityNameFromType(e => e.Name);
            builder.EntityNameFromAttribute<Db.TableAttribute>(e => e.Name);

            //
            // ColumnName getters
            //
            builder.ColumnNameFromProperty(e => e.Name);
            builder.ColumnNameFromAttribute<Db.ColumnAttribute>(e => e.Name);

            builder.ColumnKeyFromProperty(e => e.Name == "Id");
            builder.ColumnKeyFromAttribute<Db.KeyAttribute>(e => e != null);
            builder.ColumnKeyFromAttribute<Db.IdentityAttribute>(e => e != null);

            builder.ColumnIdentityFromAttribute<Db.IdentityAttribute>(e => e != null);

            return builder;
        }

        [Test]
        public void EntityInfo_Should_NotBeNull()
        {
            entityInfo.Should().NotBeNull();
        }

        [TestCase("TestModel")]
        public void EntityInfo_Name_Should_Be(String name)
        {
            entityInfo.EntityName.Should().Be(name);
        }
        
        [Test]
        public void EntityInfo_Should_Have_Columns()
        {
            entityInfo.Columns.Count().Should().Be(2);
            entityInfo.Columns.Should().NotBeEmpty();
            CollectionAssert.AllItemsAreNotNull(entityInfo.Columns);
        }
        
        [Test]
        public void ColumnName_Should_Be_Found()
        {
            var someNameInfo = entityInfo.Columns.First();
            var notSoRandomInfo = entityInfo.Columns.Skip(1).First();
            
            someNameInfo.ColumnName.Should().Be("SomeName");
            notSoRandomInfo.ColumnName.Should().Be("NotSoRandom");
        }

        [Test]
        public void Column_Should_Be_Found()
        {
            entityInfo["SomeName"].Should().NotBeNull();
        }

        [Test]
        public void Column_Should_NotBe_Found()
        {
            entityInfo["NotSoRandom"].Should().BeNull();
        }
  
        [Test]
        public void EntityInfo_Should_Have_One_Key()
        {
            entityInfo.Columns.Count(e => e.IsKey).Should().Be(1);
        }

        [Test]
        public void EntityInfo_Should_Have_One_Identity()
        {
            entityInfo.Columns.Count(e => e.IsIdentity).Should().Be(1);
        }
    }
}
