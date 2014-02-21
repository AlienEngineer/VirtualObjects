### Getting Started (Scaffolding)

What this [ScreenCast] (http://screencast.com/t/cOggCpYCqu)

This first version of the Scaffold only supports SqlServer.

```

// Creates all models based on the given data source.
Scaffold Models <SqlServer> <Database>

// Scaffold Switchs to change behavior

    -Repository						(Creates a Repository layer of abstraction)
    -TableName <TableName>			(Create the entity model of a single table)
    -NoLazyLoad						(Doesn't create virtual members)
    -ForceAnnotations				(Every field gets an annotation)
    -UsingCustomAnnotations			(Doesn't create the Annotations.cs)
	-DontConfig						(Doesn't change the .Config connection string)
	-ModelFolder <FolderName>		(By default: Models)
	-RepositoryFolder <FolderName>	(By default: Repositories)
	-AnnotationsFolder <FolderName>	(By default: Annotations)
	-ToFolder <FolderName>			(By default: Scaffolds into a folder)
```

#### What will be created and changed.

* The scaffolder will create the entity models based on the database provided. 
* If the -Repository is present a Repository layer is created. With the proper custom configuration for the entity models. And also add:
    * Annotations\Annotations.cs with all the custom attributes needed to map the entities.
    * Repositories\IRepository.cs interface.
	* Repositories\Repository.cs class that implements IRepository.
	* The proper configuration to use the custom attributes.
	* RepositoryExtensions.cs with some helpfull extensions for the new IRepository interface.
* Since we are giving it the server and the database it will change the App.Config or Web.Config connectionStrings

```

// Creates the RepositoryLayer

Scaffold Repository

```

#### What will be created and changed.

* The scaffolder will create a Repository layer with the proper custom configuration for the entity models. And also add:
    * Annotations\Annotations.cs with all the custom attributes needed to map the entities.
    * Repositories\IRepository.cs interface.
	* Repositories\Repository.cs class that implements IRepository.
	* The proper configuration to use the custom attributes.
	* RepositoryExtensions.cs with some helpfull extensions for the new IRepository interface.



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


