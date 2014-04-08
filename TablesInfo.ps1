$connection = New-Object System.Data.SqlClient.SqlConnection

function Begin-Query($server,$database)
{
    Write-Host "Server: $server"
    Write-Host "Database: $database" 

    try {
        $connectionString = "Data Source=${server};Initial Catalog=${database};Integrated Security=True"

        Write-Host $connectionString        

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
    return Get-Data ("Select * From sys.tables") | foreach {
        @{
            Name = $_.name;
            Columns = Get-Columns($_.Object_Id);
        }
    }
}

function Get-Columns($tableId) {
    $columns = @()

    Get-Data ("Select * From sys.columns Where Object_Id = $tableId")| foreach {
        $column = $_
		
        $columns += @{
            Id = $column.column_id;
            TableId = $tableId
		    Name = $column.Name;
			NameSingularized = $column.Name;
			Identity = $column.Identity;
			InPrimaryKey = $column.is_identity;
			IsForeignKey = Get-IsForeignKey $tableId $column.column_id;
		    DataType = $column.system_type_id;
		}

		# $columns.ForeignKeys = @()

	}

    return $columns
}

function Get-IsForeignKey($tableId, $columnsId) {
    return $false
}

function Print-Table($table) {
    Write-Host "------------------------------------------------"
    Write-Host "TableName   : " $table.Name
    Write-Host "Columns     : " $table.Columns.Count
    Print-Columns ($table.Columns)
    Write-Host ""
}

function Print-Columns($columns) {
    $columns | foreach {
        Write-Host "Column Name : " $_.Name
    }
}

if (Begin-Query ".\Development" "Northwind")
{
    Get-Tables | foreach {
        Print-Table $_
    }
    
    End-Query
}
