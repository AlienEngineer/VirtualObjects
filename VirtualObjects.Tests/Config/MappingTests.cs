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

            [Db.Association("ExtId", "Id")]
            public TestModel1 OtherModel { get; set; }
        }

        public class TestModel1
        {
            [Db.Key]
            public int Id { get; set; }
            
        }

        IEntityInfo _entityInfo;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _entityInfo = MapTestModel();
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
            builder.EntityNameFromAttribute<Db.TableAttribute>(e => e.TableName);

            //
            // ColumnName getters
            //
            builder.ColumnNameFromProperty(e => e.Name);
            builder.ColumnNameFromAttribute<Db.ColumnAttribute>(e => e.FieldName);

            builder.ColumnKeyFromProperty(e => e.Name == "Id");
            builder.ColumnKeyFromAttribute<Db.KeyAttribute>(e => e != null);
            builder.ColumnKeyFromAttribute<Db.IdentityAttribute>(e => e != null);

            builder.ColumnIdentityFromAttribute<Db.IdentityAttribute>(e => e != null);

            builder.ForeignKeyFromAttribute<Db.AssociationAttribute>(e => e.OtherKey);

            return builder;
        }

        [Test]
        public void EntityInfo_Should_NotBeNull()
        {
            _entityInfo.Should().NotBeNull();
        }

        [TestCase("TestModel")]
        public void EntityInfo_Name_Should_Be(String name)
        {
            _entityInfo.EntityName.Should().Be(name);
        }
        
        [Test]
        public void EntityInfo_Should_Have_Columns()
        {
            _entityInfo.Columns.Count().Should().Be(3);
            _entityInfo.Columns.Should().NotBeEmpty();
            CollectionAssert.AllItemsAreNotNull(_entityInfo.Columns);
        }
        
        [Test]
        public void ColumnName_Should_Be_Found()
        {
            var someNameInfo = _entityInfo.Columns.First();
            var notSoRandomInfo = _entityInfo.Columns.Skip(1).First();
            
            someNameInfo.ColumnName.Should().Be("SomeName");
            notSoRandomInfo.ColumnName.Should().Be("NotSoRandom");
        }

        [Test]
        public void Column_Should_Be_Found()
        {
            _entityInfo["SomeName"].Should().NotBeNull();
        }

        [Test]
        public void Column_Should_NotBe_Found()
        {
            _entityInfo["NotSoRandom"].Should().BeNull();
        }
  
        [Test]
        public void EntityInfo_Should_Have_One_Key()
        {
            _entityInfo.Columns.Count(e => e.IsKey).Should().Be(1);
        }

        [Test]
        public void EntityInfo_Should_Have_One_Identity()
        {
            _entityInfo.Columns.Count(e => e.IsIdentity).Should().Be(1);
        }

        [Test]
        public void EntityInfo_Should_Have_One_Association()
        {
            _entityInfo.Columns.Count(e => e.ForeignKey != null).Should().Be(1);
        }

        [Test]
        public void EntityInfo_ForeignKey_Should_Be()
        {
            var field = _entityInfo.Columns.First(e => e.ForeignKey != null);

            var foreignKey = field.ForeignKey;

            foreignKey.IsKey.Should().BeTrue();
            foreignKey.ColumnName.Should().Be("Id");
        }
    }
}
