<?php
//
//include 'libs/Parsedown.php';
//$Parsedown = new Parsedown();
//echo $Parsedown->fromFile('markdown/home.md');
?>


<h2>!Page Under Construction!</h2>
<h3>Getting Started (Scaffolding)</h3>

<!-- 16:9 aspect ratio -->
<!--<div class="embed-responsive embed-responsive-16by9">
    <iframe class="embed-responsive-item" src="http://youtu.be/NeUzfCv3zRw"></iframe>
</div>-->

<pre class="sunlight-highlight-csharp">
    # Creates the entity models based on data source and create a repository Layer.
    Scaffold Models ".\Development" "Northwind" -Repository

    or...

    # Creates the entity models based on data source.
    Scaffold Models ".\Development" "Northwind"

    # Create a repository Layer.
    Scaffold Repository

    # Create a business Layer. (e.g. Scaffold Business Task)
    Scaffold Business < ModelType >
</pre>
<hr>
<h3>Getting Started (Manually)</h3>
<h4>Create a Model</h4>
<pre class="sunlight-highlight-csharp">
    public class Employee 
    {
        public String EmployeeId { get; set; }
        public String LastName { get; set; }
        // Other fields ...
    }
</pre>

<h4>Config the Connection</h4>
<pre class="sunlight-highlight-xml">
    &lt;connectionStrings&gt;
        &lt;clear/&gt;
        &lt;add name="YourMachineName" 
            providerName="System.Data.SqlClient" 
            connectionString="
                Data Source=.\instance;
                Initial Catalog=db;
                Integrated Security=true"/&gt;
    &lt;/connectionStrings&gt;
</pre>

<h4>Use it!</h4>
<pre class="sunlight-highlight-csharp">
    using (var session = new Session())
    {
        IQueryable&lt;Employee&gt; employees = session.GetAll&lt;mployee&gt;()
            .Where(e => e.BirthDate.Year > 1983);

        session.Insert(new Employee 
        {
            Name = "I'm the new guy!"
        });
    }
</pre>

<h3>Why</h3>
<p>To improve myself and create something really easy to use with the best performance possible. This project started in college with ORMFramework and later VODB.</p>
<hr>
<h3>Performance</h3>
<p>Verified on unit-tests using NUnit on a intel i7 3.07GHz 18Gb RAM and intel i7-3537U 2.0GHz 8Gb RAM.</p>
<ul>
    <li>VO Version 1.3</li>
    <li>Dapper Version 1.13</li>
    <li>Entity Framework 6</li>
</ul>
<h4>Under Northwind Database</h4>
<p>To obtain a fresh Graphic and more detail execute the unit-test <em>Performance Check</em> the excel will be filled on Bin\Performance\Session folder.
    More graphics available on /docs folder.</p>

<pre class="sunlight-highlight-tsql">
    Select Count(*) from Suppiers
</pre>

<div class="row" >
    <img class="col-xs-12" alt="Count Suppliers" src="images/CountSuppliers.png">
    <blockquote>
        <p>Results from Laptop and PC were iqual.</p>
    </blockquote>
</div>

<pre class="sunlight-highlight-tsql">
    Select * from Suppiers
</pre>

<div class="row" >
    <img class="col-xs-12" alt="Mapping Suppliers" src="images/SuppliersMapping.png">
    <blockquote>
        <p>Results from intel i7-3537U 2.0GHz 8Gb RAM (4 core) Laptop</p>
    </blockquote>
</div>

<div class="row" >
    <img class="col-xs-12" alt="Mapping Suppliers" src="images/SuppliersMapping_pc.png">
    <blockquote>
        <p>Results from intel i7 3.07GHz 18Gb RAM (8 core) PC</p>
    </blockquote>    
</div>

