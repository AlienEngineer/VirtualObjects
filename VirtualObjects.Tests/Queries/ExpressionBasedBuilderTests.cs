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
        readonly IMapper mapper;
        ExpressionBasedBuilder builder;

        public ExpressionBasedBuilderTests()
        {
            var mapperMock = new Mock<IMapper>();

            mapperMock.Setup(e => e.Map(null)).Returns(MakeEntityInfo());

            mapper = mapperMock.Object;
        }

        class Person
        {
            public int Id { get; set; }
            public String Name { get; set; }
        }
  
        
        private IEntityInfo MakeEntityInfo()
        {
            var columns = new List<IEntityColumnInfo>
            {
                new EntityColumnInfo { ColumnName = "Id", IsKey = true },
                new EntityColumnInfo { ColumnName = "Name"}
            };

            return new EntityInfo
            {
                EntityName = "People",
                Columns = columns,
                KeyColumns = columns.Where(e => e.IsKey).ToList()
            };
        }

        [SetUp]
        public void SetUpEachTestMethod()
        {
            builder = new ExpressionBasedBuilder(
                new QueryCompiler(new SqlFormatter())
            );
        }


        [Test]
        public void Projection_Should_Show_Projected_Field()
        {
            builder.Project<Person>(e => new { e.Id });

            builder.BuildQuery().CommandText
                .Should().Be("Select [T0].[Id]");
        }

        [Test]
        public void Projection_Should_Show_All_Fields()
        {
            builder.Project<Person>(e => e);

            builder.BuildQuery().CommandText
                .Should().Be("Select [T0].[Id], [T0].[Name]");
        }

        [Test]
        public void Source_Should_Be_People()
        {
            builder.Project<Person>(e => e);
            builder.From<Person>();

            builder.BuildQuery().CommandText
                .Should().Be("Select [T0].[Id], [T0].[Name] From [People] [T0]");
        }

    }
}
