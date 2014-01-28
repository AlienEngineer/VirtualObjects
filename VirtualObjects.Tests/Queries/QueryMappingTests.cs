using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using FluentAssertions;
using VirtualObjects.Queries;
using VirtualObjects.Queries.Mapping;
using VirtualObjects.Tests.Models.Northwind;

namespace VirtualObjects.Tests.Queries
{
    using NUnit.Framework;

    /// <summary>
    /// 
    /// Unit-Tests for query Mapping
    /// 
    /// Author: Sérgio
    /// </summary>
    [TestFixture, Category("Entity Mapping")]
    public class QueryMappingTests : UtilityBelt
    {
        readonly IEntitiesMapper _entitiesMapper;

        public QueryMappingTests()
        {
            _entitiesMapper = new CollectionEntityMapper(Mapper,
                new EntityProvider.EntityProviderComposite(
                    new List<IEntityProvider>
                        {
                            new EntityProvider.EntityProvider(),
                            new EntityProvider.DynamicTypeProvider()
                        }
                    ),
                new List<IEntityMapper>
                {
                    new OrderedEntityMapper(),
                    new DynamicTypeEntityMapper()
                });
        }

        int _count;

        [TearDown]
        public void FlushTime()
        {
            if (!TestContext.CurrentContext.Test.Properties.Contains("Repeat"))
            {
                return;
            }

            var times = (int)TestContext.CurrentContext.Test.Properties["Repeat"];

            _count++;

            if (_count % times != 0) return;

            Diagnostic.PrintTime(TestContext.CurrentContext.Test.Name + " => Query mapping in time :   {1} ms");
        }

        /// <summary>
        /// 
        /// Description
        /// 
        /// </summary>
        [Test]
        [Repeat(REPEAT)]
        public void Manual_Query_Mapping()
        {
            var reader = Execute(Query<Employee>());
            var mapper = new OrderedEntityMapper();
            var mapperContext = new MapperContext
            {
                EntityInfo = Mapper.Map(typeof(Employee)),
                OutputType = typeof(Employee)
            };

            var entities = new List<Employee>();

            while (reader.Read())
            {
                entities.Add((Employee)mapper.MapEntity(reader, new Employee(), mapperContext));
            }

            reader.Close();

            entities.Should().NotBeEmpty();
            entities.Count.Should().Be(9);
        }

        [SetUp]
        public void SetUpConnection()
        {
            Connection.Open();
        }

        [TearDown]
        public void CleanUpConnection()
        {
            Connection.Close();
        }

        private IList<TEntity> MapEntities<TEntity>(IQueryable<TEntity> queryable)
        {
            var reader = Execute(queryable);
            return Diagnostic.Timed(() => (IList<TEntity>)_entitiesMapper.MapEntities<TEntity>(reader));
        }

        [Test, Repeat(REPEAT)]
        public void Mapper_GetAllEmployees()
        {
            var query = Query<Employee>();

            var entities = MapEntities(query);

            entities.Should().NotBeNull();
            entities.Should().NotBeEmpty();
            entities.Count().Should().Be(9);
        }

        [Test, Repeat(REPEAT)]
        public void Mapper_GetAllEmployees_Predicated()
        {
            var query = Query<Employee>().Where(e => e.EmployeeId > 0);

            var entities = MapEntities(query);

            entities.Should().NotBeNull();
            entities.Should().NotBeEmpty();
            entities.Count().Should().Be(9);
        }

        [Test, Repeat(REPEAT)]
        public void Mapper_GetAllOrders()
        {
            var query = Query<Orders>();

            var entities = MapEntities(query);

            entities.Should().NotBeNull();
            entities.Should().NotBeEmpty();
            entities.Count().Should().Be(830);
        }


        [Test, Repeat(REPEAT)]
        public void Mapper_GetAllOrders_CustomProjection()
        {
            var query = Query<Orders>().Select(e => new { e.OrderId, e.OrderDate });

            var entities = MapEntities(query);

            entities.Should().NotBeNull();
            entities.Should().NotBeEmpty();
            entities.Count().Should().Be(830);
        }

    }

}
