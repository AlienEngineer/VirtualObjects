using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using VirtualObjects.Config;
using VirtualObjects.Connections;
using VirtualObjects.Queries;
using VirtualObjects.Queries.Execution;

namespace VirtualObjects.Tests.IOC
{
    /// <summary>
    /// 
    /// IOC tests
    /// 
    /// Author: Sérgio
    /// </summary>
    [TestFixture, Category("IOC Tests")]
    public class IOCTests
    {
        

        [Test]
        public void IOC_GetResult_Should_NotBeNull()
        {
            var _container = new DryiocContainer(null, new FirstConnectionDbConnectionProvider());

            var types = new List<Type>();

            types
                .Append<IEntityProvider>()
                .Append<ISession>()
                .Append<IOperationsProvider>()
                .Append<IEntityProvider>()
                .Append<IQueryTranslator>()
                .Append<IEntitiesMapper>()
                .Append<IEntityMapper>()
                .Append<IQueryExecutor>()
                .Append<IQueryProvider>()
                .Append<IConnection>()
                .Append<SessionContext>()
                ;

            foreach (var type in types)
            {

                _container.Get(type)
                    .Should()
                    .NotBeNull();
            }
        }


    }

    static class IOCHelper
    {
        public static ICollection<Type> Append<T>(this ICollection<Type> types)
        {
            types.Add(typeof(T));
            return types;
        }
    }
}