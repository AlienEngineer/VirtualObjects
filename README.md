### Getting Started (Scaffolding)

```ps

# Creates the entity models based on data source and create a repository Layer.
Scaffold Models .\Development Northwind -Repository

or...

# Creates the entity models based on data source.
Scaffold Models .\Development Northwind

# Create a repository Layer.
Scaffold Repository

# Create a business Layer. (e.g. Scaffold Business Task)
Scaffold Business <ModelType>


```

****

### Getting Started (Manualy)

#### Create a Model
```C#
public class Employee 
{
    public String EmployeeId { get; set; }
    public String LastName { get; set; }
    // Other fields ...
}
```
#### Config the Connection
```XML
  <connectionStrings>
    <clear/>
    <add name="YourMachineName" 
         providerName="System.Data.SqlClient" 
         connectionString="Data Source=.\instance;Initial Catalog=db;Integrated Security=true"/>
  </connectionStrings>
```
#### Use it!
```C#
   using (var session = new Session())
   {
      IQueryable<Employee> employees = session.GetAll<Employee>()
        .Where(e => e.BirthDate.Year > 1983);
      
      session.Insert(new Employee 
      {
         Name = "I'm the new guy!"
      });
   }
```


### Why
To improve myself and create something really easy to use with the best performance possible. This project started in college with ORMFramework and later VODB.


### Performance
Verified on unit-tests using NUnit on a I7 intel 3.07Ghz 18Gb RAM.

The unit tests should be executed one at a time. So the assembly load time is calculated correctly for each test.
Tests are repeated 10x via NUnit RepeatAttribute. The timer ignores the very first result, since the first will be way bigger then the remaining executions.

VO Version 1.0.1-Beta

Dapper Version 1.13

#### Using Northwind Order Details table with 2155 Records.
```C#
    // VO: Iterated order details Query in ~64 ms
    Session.GetAll<OrderDetailsSimplified>()
    
    // Dapper: Iterated order details Query in ~52 ms
    Connection.Query<OrderDetailsSimplified>("Select * from [Order Details]")
```
#### On Northwind supplier table.
```C#
    // VO: Iterated the same Query 1000 times in ~410 ms
    Session.GetAll<Suppliers>()
    
    // Dapper: Iterated the same Query 1000 times in ~429 ms
    Connection.Query<Suppliers>("Select * from Suppliers")
```
```C#
    // VO: Iterated Query in ~8 ms
    Session.GetAll<Suppliers>()
    
    // Dapper: Iterated Query in ~16 ms
    Connection.Query<Suppliers>("Select * from Suppliers")
```

### For more info click [here] (http://alienengineer.github.com/VirtualObjects/)
### Get it as a NuGet Package [here] (http://www.nuget.org/packages/VirtualObjects/)
```
   PM> Install-Package VirtualObjects -Pre
```

### Dependencies
* Castle.Core       (≥ 3.2.2)
* fasterflect       (≥ 2.1.3)
* Ninject           (≥ 3.0.1.10)


