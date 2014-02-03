using FluentAssertions;
using NUnit.Framework;
using VirtualObjects.Core.CRUD;
using VirtualObjects.Queries.Formatters;
using VirtualObjects.Queries.Mapping;
using VirtualObjects.Tests.Models.Northwind;

namespace VirtualObjects.Tests.Crud
{
    [TestFixture, Category("Crud Operations")]
    public class CrudTests : UtilityBelt
    {
        readonly IOperations _operations;

        public CrudTests()
        {
            var provider = new OperationsProvider(new SqlFormatter(), new OrderedEntityMapper());
            _operations = provider.CreateOperations(Mapper.Map(typeof(Employee)));
        }

        [Test]
        public void OperationsProvider_Employee_Tests()
        {
            _operations.DeleteOperation.CommandText
                .Should().Be("Delete From [Employees] Where [EmployeeId] = @EmployeeId");

            _operations.GetOperation.CommandText
                .Should().Be("Select [EmployeeId], [LastName], [FirstName], [Title], [TitleOfCourtesy], [BirthDate], [HireDate], [Address], [City], [Region], [PostalCode], [Country], [HomePhone], [Extension], [Notes], [Photo], [ReportsTo], [PhotoPath], [Version] From [Employees] Where [EmployeeId] = @EmployeeId");

            _operations.InsertOperation.CommandText
                .Should().Be("Insert Into [Employees] ([LastName], [FirstName], [Title], [TitleOfCourtesy], [BirthDate], [HireDate], [Address], [City], [Region], [PostalCode], [Country], [HomePhone], [Extension], [Notes], [Photo], [ReportsTo], [PhotoPath]) Values (@LastName, @FirstName, @Title, @TitleOfCourtesy, @BirthDate, @HireDate, @Address, @City, @Region, @PostalCode, @Country, @HomePhone, @Extension, @Notes, @Photo, @ReportsTo, @PhotoPath)");

            _operations.UpdateOperation.CommandText
                .Should().Be("Update [Employees] Set [LastName] = @LastName, [FirstName] = @FirstName, [Title] = @Title, [TitleOfCourtesy] = @TitleOfCourtesy, [BirthDate] = @BirthDate, [HireDate] = @HireDate, [Address] = @Address, [City] = @City, [Region] = @Region, [PostalCode] = @PostalCode, [Country] = @Country, [HomePhone] = @HomePhone, [Extension] = @Extension, [Notes] = @Notes, [Photo] = @Photo, [ReportsTo] = @ReportsTo, [PhotoPath] = @PhotoPath Where [EmployeeId] = @EmployeeId");
        }

        [Test, Repeat(REPEAT)]
        public void GetOperation_Employee_Test()
        {
            var employee = _operations.GetOperation
                .PrepareOperation(new Employee
                {
                    EmployeeId = 1
                }).Execute(this) as Employee;

            employee.Should().NotBeNull();
            employee.EmployeeId.Should().Be(1);
            employee.LastName.Should().Be("Davolio");
        }

        [Test, Repeat(REPEAT)]
        public void InsertOperation_Employee_Test()
        {
            RollBackOnTearDown();

            var employee = _operations.InsertOperation
                .PrepareOperation(new Employee
                {
                    EmployeeId = 10,
                    FirstName = "Sérgio",
                    LastName = "Ferreira"
                }).Execute(this) as Employee;

            employee.Should().NotBeNull();
            employee.EmployeeId.Should().Be(1);
            employee.LastName.Should().Be("Ferreira");

        }

        [Test, Repeat(REPEAT)]
        public void DeleteOperation_Employee_Test()
        {
            RollBackOnTearDown();
        
        }

        [Test, Repeat(REPEAT)]
        public void UpdateOperation_Employee_Test()
        {
            RollBackOnTearDown();
        
        }

    }
}
