using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace VirtualObjects.Tests.Scaffold
{
    using NUnit.Framework;
    using VirtualObjects.Scaffold;

    [TestFixture, Category("VirtualObjectsHelper")]
    public class VirtualObjectsHelperTests : UtilityBelt
    {

        [Test]
        public void Helper_Can_Produce_TablesInformation()
        {
            var tables = VirtualObjectsHelper.GetTables();
            tables.Should().NotBeNull();
            tables.Should().NotBeEmpty();
        }

    }
}
