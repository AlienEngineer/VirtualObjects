$connection = New-Object System.Data.SqlClient.SqlConnection

function Begin-Query($server,$database)
{
    Write-Verbose "Server: $server"
    Write-Verbose "Database: $database" 

    try {
        $connectionString = "Data Source=${server};Initial Catalog=${database};Integrated Security=True"

        Write-Verbose $connectionString        

        $connection.ConnectionString = $connectionString
        $connection.Open()
    } catch [System.Exception] {
        Write-Error $Error[0];
        return $false
    }    
    
    return $true
}

function End-Query() {
    $connection.Close()
}

function Get-Data($query) {
    
    $command = $connection.CreateCommand();

    $command.CommandText = $query

    $reader = $command.ExecuteReader();
    $table = New-Object System.Data.DataTable
    $table.Load($reader)
    $reader.Close()

    return $table
}

function Get-Tables {
    try
    {
        $tables = Get-Data ("Select Object_Id, Name From sys.tables")

        return $tables | foreach {
            $currTable = $_

            $tableDynamic = @{
			    Name = $currTable.Name;
			    #NameSingularized = (Get-SingularizedWord $table.Name);
                Column = Get-Columns($currTable.Name)
		    }
        }
    }
    catch [Exception]
    {
        Write-Host $Error[0]
    }    
}

function Get-Columns($tableName) {
    $columns = @()

    return $columns
}

function Print-Table($table) {
    $format = @{Expression={$_.Name};Label="Table Id     ";width=15}

    $table | Format-Table $format
}

if (Begin-Query ".\Development" "Northwind")
{
    Get-Tables | foreach Print-Table $_

    End-Query
}
