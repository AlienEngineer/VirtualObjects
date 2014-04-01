using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Machine.Specifications;
using VirtualObjects.Scaffold;

namespace VirtualObjects.Tests
{
    [Subject("Scaffolding"), Tags("Database Schema")]
    public class When_gettting_a_specific_table_info  
    {
        Establish context = () => {  };

        Because of = () => { Table = VirtualObjectsHelper.GetTables("Northwind", "(LocalDB)\\v11.0", "Employees").FirstOrDefault(); };

        It should_not_be_null = () => Table.Should().NotBeNull();
        It should_have_a_name = () => Table.Name.Should().NotBeNullOrEmpty();
        It should_have_columns = () => Table.Columns.Count.Should().BeGreaterThan(0);
        It should_have_18_columns = () => Table.Columns.Count.Should().Be(18);
        It should_have_1_identity = () => Table.Columns.Count(e => e.Identity).Should().Be(1);
        It should_have_a_key_named_employeeid = () => Table.Columns.First(e => e.Identity).Name.Should().Be("EmployeeID");

        static VirtualObjectsHelper.MetaTable Table;
    }

    [Subject("Scaffolding"), Tags("Database Schema")]
    public class When_getting_all_tables
    {
        Establish context = () => {  };

        Because of = () => { Tables = VirtualObjectsHelper.GetTables("Northwind", "(LocalDB)\\v11.0"); };

        It should_not_be_empty = () => Tables.Any().Should().BeTrue();
        
        static IEnumerable<VirtualObjectsHelper.MetaTable> Tables;
    }
}
