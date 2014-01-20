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
            var builder = new AttributeMappingBuilder();

            _mapper = builder.Build();
        }

        class People
        {
            public int Id { get; set; }
            public String Name { get; set; }

            public Bosses Boss { get; set; }
        }

        class Bosses
        {
            public int Id { get; set; }
            public String Name { get; set; }
        }

        [SetUp]
        public void SetUpEachTestMethod()
        {
            _builder = new ExpressionBasedBuilder(
                new QueryCompiler(new SqlFormatter(), _mapper)
            );

            _builder.Project<People>(e => new { e.Id, e.Name });
            _builder.From<People>();
        }

        [Test]
        public void Projection_Should_Show_Projected_Field()
        {
            _builder.Project<People>(e => new { e.Id });
            _builder.From<People>();

            _builder.BuildQuery().CommandText
                .Should().Be("Select [T0].[Id] From [People] [T0]");
        }

        [Test]
        public void Source_Should_Be_People()
        {
            _builder.BuildQuery().CommandText
                .Should().Be("Select [T0].[Id], [T0].[Name] From [People] [T0]");
        }


        [Test]
        public void WhereClause_Should_Be_Id_Equals_1()
        {
            _builder.Where<People>(e => e.Id == 1);

            _builder.BuildQuery().CommandText
                .Should().Be("Select [T0].[Id], [T0].[Name] From [People] [T0] Where ([T0].[Id] = @p0)");
        }


        [Test]
        public void WhereClause_Should_Be_Id_Equals_1_Or_Id_Equals_2()
        {
            _builder.Where<People>(e => e.Id == 1 || e.Id == 2);

            _builder.BuildQuery().CommandText
                .Should().Be("Select [T0].[Id], [T0].[Name] From [People] [T0] Where (([T0].[Id] = @p0) Or ([T0].[Id] = @p1))");
        }

        [Test]
        public void WhereClause_Should_Be_Id_Equals_1_Or_Id_Equals_2_And_Id_Greater_Than_1()
        {
            _builder.Where<People>(e => e.Id == 1 || e.Id == 2);
            _builder.Where<People>(e => e.Id > 1);

            _builder.BuildQuery().CommandText
                .Should().Be("Select [T0].[Id], [T0].[Name] From [People] [T0] Where (([T0].[Id] = @p0) Or ([T0].[Id] = @p1)) And ([T0].[Id] > @p2)");
        }

        [Test]
        public void WhereClause_Should_Be_Id_Plus_1_Equals_2()
        {
            _builder.Where<People>(e => e.Id + 1 == 1);

            _builder.BuildQuery().CommandText
                .Should().Be("Select [T0].[Id], [T0].[Name] From [People] [T0] Where (([T0].[Id] + @p0) = @p1)");
        }

        [TestCase(1)]
        public void WhereClause_Should_Be_Id_Equal_Parameter(int value)
        {
            _builder.Where<People>(e => e.Id == value);

            _builder.BuildQuery().CommandText
                .Should().Be("Select [T0].[Id], [T0].[Name] From [People] [T0] Where ([T0].[Id] = @p0)");
        }

        [Test]
        public void WhereClause_Should_Be_Id_Equal_FuncResult()
        {
            var func = new Func<int>(() => 1);

            _builder.Where<People>(e => e.Id == func());

            _builder.BuildQuery().CommandText
                .Should().Be("Select [T0].[Id], [T0].[Name] From [People] [T0] Where ([T0].[Id] = @p0)");
        }

        [Test]
        public void People_With_Boss_Equals_To_1()
        {
            _builder.Where<People>(e => e.Boss.Id == 1);

            _builder.BuildQuery().CommandText
                .Should().Be("Select [T0].[Id], [T0].[Name] From [People] [T0] Where [T0].[Id] In (Select [T1].[Id] From [Bosses] [T1] Where [T1].[Id] = @p0)");
        }


    }
}
