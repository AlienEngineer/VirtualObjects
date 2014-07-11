### Getting Started (Scaffolding)

[Short demonstration video] (http://screencast.com/t/CghgJTdmx#mediaDisplayArea)

```
# Creates the entity models based on data source and create a repository Layer.
Scaffold Models ".\Development" "Northwind" -Repository

or...

# Creates the entity models based on data source.
Scaffold Models ".\Development" "Northwind"

# Create a repository Layer.
Scaffold Repository

# Create a business Layer. (e.g. Scaffold Business Task)
Scaffold Business <ModelType>
```

****

### Getting Started (Manually)

#### Create a Model
```
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
         connectionString="
            Data Source=.\instance;
            Initial Catalog=db;
            Integrated Security=true"/>
  </connectionStrings>
```
#### Use it!
```
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
Verified on unit-tests using NUnit on a intel i7 3.07GHz 18Gb RAM and intel i7-3537U 2.0GHz 8Gb RAM.


* VO Version 1.3
* Dapper Version 1.13
* Entity Framework 6

#### Under Northwind Database
To obtain a fresh Graphic and more detail execute the unit-test _Performance Check_ the excel will be filled on Bin\Performance\Session folder.
More graphics will be provided on /docs folder.

```MSSQL
Select Count(*) from Suppliers
```
![Count Suppliers](images/CountSuppliers.png)
> Results from Laptop and PC were iqual.

```MSSQL
Select * from Suppliers
```

![Mapping Suppliers] (images/SuppliersMapping.png) 

> Results from intel i7-3537U 2.0GHz 8Gb RAM (4 core) Laptop

![Mapping Suppliers] (images/SuppliersMapping_pc.png) 

> Results from intel i7 3.07GHz 18Gb RAM (8 core) PC

***
