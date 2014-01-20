using System;
using System.Linq;
using FluentAssertions;

namespace VirtualObjects.Tests.Queries
{
    using Moq;
    using NUnit.Framework;
    using System.Collections.Generic;
    using VirtualObjects.Config;
    using VirtualObjects.Queries.Builder;
    using VirtualObjects.Queries.Compilation;
    using VirtualObjects.Queries.Formatters;

    /// <summary>
    /// 
    /// Unit-tests for query building
    /// 
    /// Author: Sérgio
    /// </summary>
    [TestFixture, Category("Query Building")]
    public class ExpressionBasedBuilderTests 
    {
        readonly IMapper _mapper;
        ExpressionBasedBuilder _builder;

        public ExpressionBasedBuilderTests()
        {
            var mapperMock = new Mock<IMapper>();

            mapperMock.Setup(e => e.Map(typeof(Person))).Returns(MakeEntityInfo());

            _mapper = mapperMock.Object;
        }

        class Person
        {
            public int Id { get; set; }
            public String FullName { get; set; }
        }
  
        
        private static IEntityInfo MakeEntityInfo()
        {
            var personType = typeof (Person);

            var columns = new List<IEntityColumnInfo>
            {
                new EntityColumnInfo { ColumnName = "Id", IsKey = true, Property = personType.GetProperty("Id")},
                new EntityColumnInfo { ColumnName = "Name", Property = personType.GetProperty("FullName")}
            };

            return new EntityInfo
            {
                EntityName = "People",
                EntityType = personType,
                Columns = columns,
                KeyColumns = columns.Where(e => e.IsKey).ToList()
            };
        }

        [SetUp]
        public void SetUpEachTestMethod()
        {
            _builder = new ExpressionBasedBuilder(
                new QueryCompiler(new SqlFormatter(), _mapper)
            );
        }

        [Test]
        public void Projection_Should_Show_Projected_Field()
        {
            _builder.Project<Person>(e => new { e.Id });
            _builder.From<Person>();

            _builder.BuildQuery().CommandText
                .Should().Be("Select [T0].[Id] From [People] [T0]");
        }
        
        [Test]
        public void Source_Should_Be_People()
        {
            _builder.Project<Person>(e => e);
            _builder.From<Person>();

            _builder.BuildQuery().CommandText
                .Should().Be("Select [T0].[Id], [T0].[Name] From [People] [T0]");
        }

    }
}
