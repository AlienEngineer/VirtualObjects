using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using VirtualObjects.Mappings;

namespace VirtualObjects.Tests.Config
{
    /// <summary>
    /// 
    /// Unit-tests for MappingBuilder and Mapper
    /// 
    /// Author: Sérgio
    /// </summary>
    [TestFixture, Category("Mapping")]
    public class MappingTests : UtilityBelt
    {
        public class TestModel
        {
            public int SomeName { get; set; }
            
            [Identity(FieldName = "NotSoRandom")]
            public int SomeRandomName { get; set; }

            [Association(FieldName = "ExtId", OtherKey = "Id")]
            public TestModel1 OtherModel { get; set; }

            [Association(FieldName = "ExtId", OtherKey = "SomeRandomName")]
            public TestModel OtherModelKey { get; set; }

            [Format(Format = "yyyy-MM-dd")]
            public DateTime Date { get; set; }

            [NumberFormat(DecimalSeparator = ",", GroupSeparator = ".", GroupSizes = 3)]
            public double SomeValue { get; set; }
        }

        public class TestModel1
        {
            [Key]
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
            return Mapper.Map(typeof(TestModel));
        }
  
        [Test]
        public void EntityInfo_Should_NotBeNull()
        {
            _entityInfo.Should().NotBeNull();
        }

        [TestCase("TestModel")]
        public void EntityInfo_Name_Should_Be(string name)
        {
            _entityInfo.EntityName.Should().Be(name);
        }
        
        [Test]
        public void EntityInfo_Should_Have_Columns()
        {
            _entityInfo.Columns.Count().Should().Be(6);
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
        public void EntityInfo_Should_Have_Specific_Format_On_Field()
        {
            _entityInfo.Columns.Count(e => e.HasFormattingStyles && e.Formats.Contains("yyyy-MM-dd")).Should().Be(1);
        }

        [Test]
        public void EntityInfo_Should_Have_Specific_Number_Format_On_Field()
        {
            _entityInfo.Columns.Count(e => e.HasFormattingStyles && e.NumberFormat != null && e.NumberFormat.NumberDecimalSeparator == ",").Should().Be(1);
        }

        [Test]
        public void EntityInfo_Should_Have_Two_Associations()
        {
            _entityInfo.Columns.Count(e => e.ForeignKey != null).Should().Be(2);
        }

        [Test]
        public void EntityInfo_ForeignKey_Should_Be()
        {
            var field = _entityInfo.Columns.First(e => e.ForeignKey != null);

            var foreignKey = field.ForeignKey;

            foreignKey.IsKey.Should().BeTrue();
            foreignKey.ColumnName.Should().Be("Id");
        }

        [Test]
        public void EntityInfo_Other_ForeignKey_Should_Be()
        {
            var field = _entityInfo.Columns.Where(e => e.ForeignKey != null).Skip(1).First();

            var foreignKey = field.ForeignKey;

            foreignKey.IsKey.Should().BeTrue();
            foreignKey.ColumnName.Should().Be("NotSoRandom");
        }
    }
}
