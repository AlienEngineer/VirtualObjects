### Getting Started (Scaffolding)

[Short demonstration video] (http://screencast.com/t/CghgJTdmx#mediaDisplayArea)

```
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

### Getting Started (Manually)

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

***

### Performance
Verified on unit-tests using NUnit on a I7 intel 3.07Ghz 18Gb RAM.


* VO Version 1.1
* Dapper Version 1.13
* Entity Framework 6

#### Under Northwind Database
To obtain a fresh Graphic and more detail execute the unit-test _Performance_With_ExcelRecords_ the excel will be filled on Bin\Release\Session or Bin\Debug\Session directory.

```MySQL
Select Count(*) from Suppliers
```
![Count Suppliers](https://raw.githubusercontent.com/AlienEngineer/VirtualObjects/ParallelMapping/Docs/CountSuppliers.png)
> Results in milliseconds.  
> The lower the better.  
> The first execution is excluded. Including it results in a unreadable graphic.

```MySQL
Select * from Suppliers
```
![Mapping Suppliers] (https://raw.githubusercontent.com/AlienEngineer/VirtualObjects/ParallelMapping/Docs/MappingSuppliers.png)
> Results in milliseconds.  
> The lower the better.  
> The first execution is excluded. Including it results in a unreadable graphic.

***

### For more info click [here] (http://alienengineer.github.com/VirtualObjects/)
### Get it as a NuGet Package [here] (http://www.nuget.org/packages/VirtualObjects/)
```
   PM> Install-Package VirtualObjects
```
