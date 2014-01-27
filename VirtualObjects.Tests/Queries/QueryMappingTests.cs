using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualObjects.Tests.Queries
{
    using NUnit.Framework;

    /// <summary>
    /// 
    /// Unit-Tests for query Mapping
    /// 
    /// Author: Sérgio
    /// </summary>
    [TestFixture]
    public class QueryMappingTests : UtilityBelt
    {



        int _count;

        [TearDown]
        public void FlushTime()
        {
            if ( !TestContext.CurrentContext.Test.Properties.Contains("Repeat") )
            {
                return;
            }

            var times = (int)TestContext.CurrentContext.Test.Properties["Repeat"];

            _count++;

            if ( _count % times != 0 ) return;

            Diagnostic.PrintTime(TestContext.CurrentContext.Test.Name + " => Query mapping parsed in time :   {1} ms", "QueryMapping");
        }

        /// <summary>
        /// 
        /// Description
        /// 
        /// </summary>
        [Test] [Repeat(REPEAT)]
        public void TestName()
        {
            
        }

    }

}
