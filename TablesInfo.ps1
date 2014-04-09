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

function Get-Data {
    
    Param(
        [string]$query
    )
    
    $command = $connection.CreateCommand();

    $command.CommandText = $query

    $reader = $command.ExecuteReader();
    $table = New-Object System.Data.DataTable
    $table.Load($reader)
    $reader.Close()

    return $table
}

function Get-Tables($tableName) {
    #Write-Host $tableName

    $query = [string]"Select * From sys.tables where Name = '$tableName' or '$tableName' = '-'"

    #Write-Host $query

    return (Get-Data $query) | foreach {
        #Write-Host "Runnig " $_.name;

        @{
            Name = $_.name;
            Columns = Get-Columns($_.Object_Id);
        }
    }
}

function Get-Columns($tableId) {
    $columns = @()

    (Get-Data "Select * From sys.columns Where Object_Id = $tableId")| foreach {
        $column = $_
		
        $columns += @{
            Id = $column.column_id;
            TableId = $tableId
		    Name = $column.Name;
			NameSingularized = $column.Name;
			Identity = $column.is_identity;
			InPrimaryKey = Get-IsPrimaryKey $tableId $column.column_id;
			IsForeignKey = Get-IsForeignKey $tableId $column.column_id;
		    DataType = $column.system_type_id;
		}

		# $columns.ForeignKeys = @()

	}

    return $columns
}

function Get-IsPrimaryKey($tableId, $columnId) {
    
    # Write-Host "TableId  : " $tableId
    # Write-Host "ColumnId : " $columnId

    $query = [string]"Select I.is_primary_key From sys.Index_Columns C Inner Join sys.indexes I On (I.index_id = C.index_id and C.Object_Id = I.Object_Id)  Where C.Object_Id = $tableId and C.Column_Id = $columnId and I.is_primary_key = 1"

    #Write-Host $query

    $result = (Get-Data $query )

    return $result.is_primary_key -eq $true
}

function Get-IsForeignKey($tableId, $columnId) {
    return $false
}

function Print-Table($table) {
    Write-Host "------------------------------------------------"
    Write-Host ("TableName   : " + $table.Name)
    Write-Host ("Columns     : " + $table.Columns.Count)
    Print-Columns ($table.Columns)
    Write-Host ""
}

function Print-Columns($columns) {
    $columns | foreach {
        Write-Host ("Column Name : " + $_.Name + " Is Key : " + $_.InPrimaryKey)
    }
}

if (Begin-Query ".\Development" "Northwind")
{
    (Get-Tables "Employees") | foreach {
        Print-Table $_
    }
    
    End-Query
}
