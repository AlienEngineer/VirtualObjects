
using System.Diagnostics;
using FluentAssertions;
using NUnit.Framework;
using VirtualObjects.Core.CRUD;
using VirtualObjects.Queries.Formatters;
using VirtualObjects.Queries.Mapping;
using VirtualObjects.Tests.Models.Northwind;

namespace VirtualObjects.Tests.Crud
{
    [TestFixture]
    public class CrusTests : UtilityBelt
    {
        
        [Test]
        public void OperationsProvider_Employee_Tests()
        {

            var provider = new OperationsProvider(new SqlFormatter(), new OrderedEntityMapper());

            var operations = provider.CreateOperations(Mapper.Map(typeof (Employee)));

            operations.DeleteOperation.CommandText
                .Should().Be("Delete From [Employees] Where [EmployeeId] = @EmployeeId");

            operations.GetOperation.CommandText
                .Should().Be("Select [EmployeeId], [LastName], [FirstName], [Title], [TitleOfCourtesy], [BirthDate], [HireDate], [Address], [City], [Region], [PostalCode], [Country], [HomePhone], [Extension], [Notes], [Photo], [ReportsTo], [PhotoPath], [Version] From [Employees] Where [EmployeeId] = @EmployeeId");

            operations.InsertOperation.CommandText
                .Should().Be("Insert Into [Employees] ([EmployeeId], [LastName], [FirstName], [Title], [TitleOfCourtesy], [BirthDate], [HireDate], [Address], [City], [Region], [PostalCode], [Country], [HomePhone], [Extension], [Notes], [Photo], [ReportsTo], [PhotoPath], [Version]) Values (@LastName, @FirstName, @Title, @TitleOfCourtesy, @BirthDate, @HireDate, @Address, @City, @Region, @PostalCode, @Country, @HomePhone, @Extension, @Notes, @Photo, @ReportsTo, @PhotoPath, @Version)");

            operations.UpdateOperation.CommandText
                .Should().Be("Update [Employees] Set [LastName] = @LastName, [FirstName] = @FirstName, [Title] = @Title, [TitleOfCourtesy] = @TitleOfCourtesy, [BirthDate] = @BirthDate, [HireDate] = @HireDate, [Address] = @Address, [City] = @City, [Region] = @Region, [PostalCode] = @PostalCode, [Country] = @Country, [HomePhone] = @HomePhone, [Extension] = @Extension, [Notes] = @Notes, [Photo] = @Photo, [ReportsTo] = @ReportsTo, [PhotoPath] = @PhotoPath, [Version] = @Version Where [EmployeeId] = @EmployeeId");

        }


    }
}
