using System.Data.SqlClient;
using FluentAssertions;
using NUnit.Framework;
using VirtualObjects.Tests.Models.Northwind;
using VirtualObjects.CRUD;
using VirtualObjects.Queries.Formatters;
using VirtualObjects.Queries.Mapping;
using VirtualObjects.EntityProvider;

namespace VirtualObjects.Tests.Crud
{
    [TestFixture, Category("Crud Operations")]
    public class CrudTests : UtilityBelt
    {
        readonly IOperations _operations;

        public CrudTests()
        {
            var provider = new OperationsProvider(new SqlFormatter(), new OrderedEntityMapper(), new EntityModelProvider());
            _operations = provider.CreateOperations(Mapper.Map(typeof(Employee)));
        }

        [Test]
        public void OperationsProvider_Employee_Tests()
        {
            _operations.DeleteOperation.CommandText
                .Should().Be("Delete From [Employees] Where [EmployeeId] = @EmployeeId");

            _operations.GetOperation.CommandText
                .Should().Be("Select [EmployeeId], [LastName], [FirstName], [Title], [TitleOfCourtesy], [BirthDate], [HireDate], [Address], [City], [Region], [PostalCode], [Country], [HomePhone], [Extension], [Notes], [Photo], [ReportsTo], [PhotoPath], [Version] From [Employees] Where [EmployeeId] = @EmployeeId");

            _operations.GetVersionOperation.CommandText
                .Should().Be("Select [Version] From [Employees] Where [EmployeeId] = @EmployeeId");

            _operations.InsertOperation.CommandText
                .Should().Be("Insert Into [Employees] ([LastName], [FirstName], [Title], [TitleOfCourtesy], [BirthDate], [HireDate], [Address], [City], [Region], [PostalCode], [Country], [HomePhone], [Extension], [Notes], [Photo], [ReportsTo], [PhotoPath]) Values (@LastName, @FirstName, @Title, @TitleOfCourtesy, @BirthDate, @HireDate, @Address, @City, @Region, @PostalCode, @Country, @HomePhone, @Extension, @Notes, @Photo, @ReportsTo, @PhotoPath) Select @@IDENTITY");

            _operations.UpdateOperation.CommandText
                .Should().Be("Update [Employees] Set [LastName] = @LastName, [FirstName] = @FirstName, [Title] = @Title, [TitleOfCourtesy] = @TitleOfCourtesy, [BirthDate] = @BirthDate, [HireDate] = @HireDate, [Address] = @Address, [City] = @City, [Region] = @Region, [PostalCode] = @PostalCode, [Country] = @Country, [HomePhone] = @HomePhone, [Extension] = @Extension, [Notes] = @Notes, [Photo] = @Photo, [ReportsTo] = @ReportsTo, [PhotoPath] = @PhotoPath Where [EmployeeId] = @EmployeeId");
        }

        private TResult Execute<TResult>(IOperation operation, TResult entity)
        {

            return Diagnostic.Timed(() => 
                (TResult)operation.PrepareOperation(entity).Execute(SessionContext)
                );
        }


        [Test, Repeat(Repeat)]
        public void GetVersionOperation_Employee_Test()
        {
            var version = _operations.GetVersionOperation.PrepareOperation(new Employee
            {
                EmployeeId = 1
            }).Execute(SessionContext);

            version.Should().NotBeNull();
        }

        [Test, Repeat(Repeat)]
        public void GetOperation_Employee_Test()
        {
            var employee = Execute(_operations.GetOperation, new Employee
            {
                EmployeeId = 1
            });

            employee.Should().NotBeNull();
            employee.EmployeeId.Should().Be(1);
            employee.LastName.Should().Be("Davolio");
        }

        [Test, Repeat(Repeat)]
        public void InsertOperation_Employee_Test()
        {
            RollBackOnTearDown();

            var employee = Execute(_operations.InsertOperation, new Employee
            {
                EmployeeId = 10,
                FirstName = "Sérgio",
                LastName = "Ferreira"
            });

            employee.Should().NotBeNull();
            employee.EmployeeId.Should().BeGreaterThan(9);
            employee.LastName.Should().Be("Ferreira");

        }

        [Test, Repeat(Repeat), ExpectedException(typeof(SqlException))]
        public void DeleteOperation_Employee_Test()
        {
            RollBackOnTearDown();

            var employee = Execute(_operations.DeleteOperation, new Employee
            {
                EmployeeId = 5
            });

            employee.Should().NotBeNull();
            employee.EmployeeId.Should().Be(1);
        }

        [Test, Repeat(Repeat)]
        public void UpdateOperation_Employee_Test()
        {
            RollBackOnTearDown();

            var employee = Execute(_operations.UpdateOperation, new Employee
            {
                EmployeeId = 1,
                FirstName = "Sérgio",
                LastName = "Ferreira",
                Version = new byte[] { 9, 9, 9, 9, 9, 9, 9, 9 }
            });

            employee.Should().NotBeNull();
            employee.EmployeeId.Should().Be(1);
        }

    }
}
