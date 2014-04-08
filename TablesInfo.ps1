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
            Name = $_.name
        }
    }
}

function Get-Columns($tableName) {
    $columns = @()

    return $columns
}

function Print-Table($table) {
    Write-Host "------------------------------------------------"
    Write-Host "TableName : " $table.Name
    Write-Host "Columns : "
    Print-Columns ($table.Columns)
    Write-Host ""
}

function Print-Columns($columns) {
    
}

if (Begin-Query ".\Development" "Northwind")
{
    Get-Tables | foreach {
        Print-Table $_
    }
    
    End-Query
}
