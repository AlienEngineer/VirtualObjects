using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Machine.Specifications;
using Machine.Specifications.Model;
using NUnit.Framework;
using VirtualObjects.Scaffold;

namespace VirtualObjects.Tests
{
    [Subject("Scaffolding"), Tags("Database Schema")]
    public class When_gettting_a_specific_table_info  
    {
        Establish context = () => {  };

        Because of = () => { Table = VirtualObjectsHelper.GetTables("gidw01", ".\\development", "FAC_ICDO").FirstOrDefault(); };

        It should_not_be_null = () => Table.Should().NotBeNull();
        It should_have_a_name = () => Table.Name.Should().NotBeNullOrEmpty();
        It should_have_columns = () => Table.Columns.Count.Should().BeGreaterThan(0);
        It should_have_7_columns = () => Table.Columns.Count.Should().Be(7);

        static VirtualObjectsHelper.MetaTable Table;
    }

    [Subject("Scaffolding"), Tags("Database Schema")]
    public class When_Context
    {
        Establish context = () => {  };

        Because of = () => { Tables = VirtualObjectsHelper.GetTables("gidw01", ".\\development"); };

        It should_not_be_empty = () => Tables.Any().Should().BeTrue();
        
        static IEnumerable<VirtualObjectsHelper.MetaTable> Tables;
    }
}
