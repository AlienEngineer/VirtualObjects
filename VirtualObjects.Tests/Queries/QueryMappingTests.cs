﻿using System.Collections.Generic;
using System.Linq;
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
            _entitiesMapper = new CollectionEntitiesMapper(Mapper,
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
                    new DynamicTypeEntityMapper(),
                    new DynamicEntityMapper(),
                    new DynamicWithMemberEntityMapper()
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
            var queryInfo = TranslateQuery(Query<Employee>());
            var reader = Execute(queryInfo);
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
            var queryInfo = TranslateQuery(queryable);
            var reader = Execute(queryInfo);
            return Diagnostic.Timed(() => (IList<TEntity>)_entitiesMapper.MapEntities<TEntity>(reader, queryInfo));
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

        [Test, Repeat(REPEAT)]
        public void Mapper_GetAllOrders_Joined_Query_AllFields()
        {
            var query = from o in Query<Orders>()
                        join od in Query<OrderDetails>() on o equals od.Order
                        select new { Order = o, Detail = od };

            var entities = MapEntities(query);

            entities.Should().NotBeNull();
            entities.Should().NotBeEmpty();
            entities.Count().Should().Be(2155);
        }

        [Test, Repeat(REPEAT)]
        public void Mapper_GetAllOrders_Joined_Query_CustomProjection()
        {
            var query = from o in Query<Orders>()
                        join od in Query<OrderDetails>() on o equals od.Order
                        select new { o.OrderId, od.UnitPrice, od.Quantity, o.ShipName };

            var entities = MapEntities(query);

            entities.Should().NotBeNull();
            entities.Should().NotBeEmpty();
            entities.Count().Should().Be(2155);
        }


        [Test, Repeat(REPEAT)]
        public void Mapper_GetAllOrders_Joined_Query_CustomProjection_With_ForeignKey()
        {
            var query = from o in Query<Orders>()
                        join od in Query<OrderDetails>() on o equals od.Order
                        select new { o.OrderId, od.UnitPrice, od.Quantity, o.ShipName, o.Employee };

            var entities = MapEntities(query);

            entities.Should().NotBeNull();
            entities.Should().NotBeEmpty();
            entities.Count().Should().Be(2155);

            entities.All(e => e.Employee.EmployeeId == 1)
                .Should().BeFalse();

        }

        [Test, Repeat(REPEAT)]
        public void Mapper_GetAllOrders_Joined_Query_CustomProjection_With_ForeignKey_in_join()
        {
            var query = from o in Query<Orders>()
                        join od in Query<OrderDetails>() on o equals od.Order
                        join e in Query<Employee>() on o.Employee equals e
                        select new { o.OrderId, od.UnitPrice, od.Quantity, o.ShipName, Employee = e };

            var entities = MapEntities(query);

            entities.Should().NotBeNull();
            entities.Should().NotBeEmpty();
            entities.Count().Should().Be(2155);

            entities.All(e => e.Employee.EmployeeId == 1)
                .Should().BeFalse();

        }


        [Test, Repeat(REPEAT)]
        public void Mapper_GetAllOrders_Joined_Query_CustomProjection_With_ForeignKeyField_in_join()
        {
            var query = from o in Query<Orders>()
                        join od in Query<OrderDetails>() on o equals od.Order
                        join e in Query<Employee>() on o.Employee equals e
                        select new { o.OrderId, od.UnitPrice, od.Quantity, o.ShipName, Employee = e.FirstName };

            var entities = MapEntities(query);

            entities.Should().NotBeNull();
            entities.Should().NotBeEmpty();
            entities.Count().Should().Be(2155);
            
        }
    }

}
