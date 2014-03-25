using System.IO;
using System.Linq;
using FluentAssertions;
using Machine.Specifications;
using VirtualObjects.Config;
using VirtualObjects.Scaffold;
using VirtualObjects.Tests.Models.Northwind;

namespace VirtualObjects.Tests.Config
{


    [Subject(typeof(IMapper)), Tags("EntityInfo Mapper")]
    public class When_mapping_the_employee_entity : SpecsUtilityBelt
    {
        Because of = () => { EmployeeEntityInfo = Mapper.Map<Employee>(); };

        It should_not_be_null =
            () => EmployeeEntityInfo.Should().NotBeNull();

        It should_have_a_matching_dll =
            () => File.Exists(System.IO.Path.GetTempPath() + "VirtualObjects\\Internal_Builder_Employee.dll").Should().BeTrue();

        It should_be_able_to_create_entities =
            () => EmployeeEntityInfo.EntityFactory().Should().NotBeNull();

        It should_be_able_to_create_proxy =
            () => EmployeeEntityInfo.EntityProxyFactory(Session).Should().NotBeNull();

#if DEBUG
        It should_have_a_cs_file = () => File.Exists(System.IO.Path.GetTempPath() + "VirtualObjects\\Internal_Builder_Employee.dll.cs");
#endif

        static IEntityInfo EmployeeEntityInfo;
    }

    [Subject(typeof(IMapper)), Tags("EntityInfo Mapper")]
    public class When_mapping_the_foreignkey_entity : SpecsUtilityBelt
    {
        Because of = () =>
                     {
                         ForeignKeyEntityInfo = Mapper.Map<VirtualObjectsHelper.ForeingKey>();
                         ColumnField = ForeignKeyEntityInfo["Column"];
                         ReferecedField = ForeignKeyEntityInfo["ReferencedColumn"];
                     };

        It should_not_be_null =
            () => ForeignKeyEntityInfo.Should().NotBeNull();

        It should_have_a_column_field =
            () => ColumnField.Should().NotBeNull();

        It should_have_the_column_related_to_table_field =
            () =>
            {
                ColumnField.ForeignKeyLinks.Count.Should().Be(1);
                ColumnField.ForeignKeyLinks.First().Should().Be(ForeignKeyEntityInfo["Table"]);
                ColumnField.ForeignKeyLinks.First().Property.Name.Should().Be("Table");
            };

        It should_have_a_refereced_column_field = 
            () => ReferecedField.Should().NotBeNull();

        It should_have_the_refereced_column_field_related_to_refereced_table_field =
            () =>
            {
                ReferecedField.ForeignKeyLinks.Count.Should().Be(1);
                ReferecedField.ForeignKeyLinks.First().Should().Be(ForeignKeyEntityInfo["ReferencedTable"]);
                ReferecedField.ForeignKeyLinks.First().Property.Name.Should().Be("ReferencedTable");
            };

        It should_have_a_matching_dll =
            () => File.Exists(System.IO.Path.GetTempPath() + "VirtualObjects\\Internal_Builder_ForeingKey.dll").Should().BeTrue();

        It should_be_able_to_create_entities =
            () => ForeignKeyEntityInfo.EntityFactory().Should().NotBeNull();

        It should_be_able_to_create_proxy =
            () => ForeignKeyEntityInfo.EntityProxyFactory(Session).Should().NotBeNull();

        static IEntityInfo ForeignKeyEntityInfo;
        static IEntityColumnInfo ColumnField;
        static IEntityColumnInfo ReferecedField;
    }
}