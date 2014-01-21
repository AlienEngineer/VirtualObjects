using System;
using FluentAssertions;
using NUnit.Framework.Constraints;
using VirtualObjects.Tests.Config;

namespace VirtualObjects.Tests.Queries
{
    using NUnit.Framework;
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
    public class ExpressionBasedBuilderTests : UtilityBelt
    {
        ExpressionBasedBuilder _builder;

        public ExpressionBasedBuilderTests()
        {
        }

        class People
        {
            [Db.Key]
            public int Id { get; set; }
            public String Name { get; set; }
            public DateTime BirthDate { get; set; }
            public Boolean IsDeveloper { get; set; }

            public Bosses Boss { get; set; }
        }

        class Bosses
        {
            [Db.Key]
            public int BossId { get; set; }
            public String BossName { get; set; }
        }

        [SetUp]
        public void SetUpEachTestMethod()
        {
            _builder = new ExpressionBasedBuilder(
                new QueryCompiler(new SqlFormatter(), Mapper)
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
        public void People_With_BossId_Equals_To_1()
        {
            _builder.Where<People>(e => e.Boss.BossId == 1);

            _builder.BuildQuery().CommandText
                .Should().Be("Select [T0].[Id], [T0].[Name] From [People] [T0] Where ([T0].[Boss] In (Select [T1].[BossId] From [Bosses] [T1] Where [T1].[BossId] = @p0))");
        }

        [Test]
        public void People_With_Boss_Equals_To_1()
        {
            _builder.Where<People>(e => e.Boss == new Bosses { BossId = 1 });

            _builder.BuildQuery().CommandText
                .Should().Be("Select [T0].[Id], [T0].[Name] From [People] [T0] Where ([T0].[Boss] = @p0)");
        }

        [Test]
        public void People_With_BirthDate_Equals_To_New_Date()
        {
            _builder.Where<People>(e => e.BirthDate == new DateTime(DateTime.Now.Ticks));

            _builder.BuildQuery().CommandText
                .Should().Be("Select [T0].[Id], [T0].[Name] From [People] [T0] Where ([T0].[BirthDate] = @p0)");
        }

        [Test]
        public void People_With_BirthDate_Equals_To_New_Date_Default()
        {
            _builder.Where<People>(e => e.BirthDate == new DateTime());

            _builder.BuildQuery().CommandText
                .Should().Be("Select [T0].[Id], [T0].[Name] From [People] [T0] Where ([T0].[BirthDate] = @p0)");
        }


        [Test]
        public void People_Is_Developer()
        {
            _builder.Where<People>(e => e.IsDeveloper);

            _builder.BuildQuery().CommandText
                .Should().Be("Select [T0].[Id], [T0].[Name] From [People] [T0] Where ([T0].[IsDeveloper] = @p0)");
        }

        [Test]
        public void People_Not_Is_Developer()
        {
            _builder.Where<People>(e => !e.IsDeveloper);

            _builder.BuildQuery().CommandText
                .Should().Be("Select [T0].[Id], [T0].[Name] From [People] [T0] Where ([T0].[IsDeveloper] != @p0)");
        }

        [Test]
        public void People_Is_Not_Developer()
        {
            _builder.Where<People>(e => e.IsDeveloper != false);

            _builder.BuildQuery().CommandText
                .Should().Be("Select [T0].[Id], [T0].[Name] From [People] [T0] Where ([T0].[IsDeveloper] != @p0)");
        }
    }
}
