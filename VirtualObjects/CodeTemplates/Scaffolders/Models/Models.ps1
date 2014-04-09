[T4Scaffolding.Scaffolder(Description = "Creation of entity models, and a repository layer.")][CmdletBinding()]
param(       
	[parameter(Mandatory=$true, ValueFromPipelineByPropertyName = $true)][System.String]$ServerName, 
	[parameter(Mandatory=$true, ValueFromPipelineByPropertyName = $true)][System.String]$DatabaseName, 
    [System.String]$Project,
	[System.String]$CodeLanguage,
	[System.String[]]$TemplateFolders,
	[System.Management.Automation.SwitchParameter]$Force = $false,
	[System.Management.Automation.SwitchParameter]$Repository = $false,
	[System.Management.Automation.SwitchParameter]$NoLazyLoad = $false,
	[System.Management.Automation.SwitchParameter]$WithAnnotations = $false,
	[System.Management.Automation.SwitchParameter]$DontConfig = $false,
	[System.Management.Automation.SwitchParameter]$DefaultAttributes = $false,
	[System.Management.Automation.SwitchParameter]$UsingCustomAnnotations = $false,
	[System.String]$TableName = "-",
	[System.String]$ModelFolder = "Models",
	[System.String]$RepositoryFolder = "Repositories",
	[System.String]$AnnotationsFolder = "Annotations",
	[System.String]$ToFolder = "-"
)

if (-not ($ToFolder -eq "-"))
{
	if ($Repository)
	{
		$ModelFolder =  "$ToFolder\$ModelFolder"
		$RepositoryFolder =  "$ToFolder\$RepositoryFolder"
		$AnnotationsFolder =  "$ToFolder\$AnnotationsFolder"
	}
	else 
	{
		$ModelFolder =  "$ToFolder"
	}
}

$namespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value

#region Connection
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

	function Get-Tables($tableName) {
		#Write-Host $tableName

		$query = [string]"Select * From sys.tables where Name = '$tableName' or '$tableName' = '-'"

		#Write-Host $query

		return (Get-Data $query) | foreach {
			#Write-Host "Runnig " $_.name;

			@{
				Name = $_.name;
				NameSingularized = (Get-SingularizedWord $_.name);
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
		Write-Verbose "------------------------------------------------"
		Write-Verbose ("TableName   : " + $table.Name)
		Write-Verbose ("Columns     : " + $table.Columns.Count)
		Print-Columns ($table.Columns)
		Write-Verbose ""
	}

	function Print-Columns($columns) {
		$columns | foreach {
			Write-Verbose ("Column Name : " + $_.Name + " Is Key : " + $_.InPrimaryKey)
		}
	}
#endregion

try
{

	if($Repository) {

		if ($Force)
		{
			Invoke-Scaffolder Repository -Force `
				-Project $Project `
				-CodeLanguage $CodeLanguage `
				-ModelFolder $ModelFolder `
				-RepositoryFolder $RepositoryFolder `
				-AnnotationsFolder $AnnotationsFolder
		}
		else 
		{
			Invoke-Scaffolder Repository -Project $Project -CodeLanguage $CodeLanguage -ModelFolder $ModelFolder -RepositoryFolder $RepositoryFolder -AnnotationsFolder $AnnotationsFolder -UsingCustomAnnotations $UsingCustomAnnotations
		}
	}

	Write-Verbose "Getting tables info..."


	if (Begin-Query $ServerName $DatabaseName)
	{
		Get-Tables $TableName  | foreach {
			$table = $_
			$TableName = $table.Name;
			Write-Verbose ("Creating model for : " + $table.Name)
			
			Print-Table $table

			$outputPath = ("$ModelFolder\" + (Get-SingularizedWord $table.Name))

			Add-ProjectItemViaTemplate $outputPath -Template EntityTemplate `
				-Model @{ 
					Namespace = $namespace; 
					Table = $table 
					AnnotationsFolder = $AnnotationsFolder; 
					RepositoryFolder = $RepositoryFolder;
					ModelFolder = $ModelFolder;
					ForceAnnotations = [System.Boolean]$WithAnnotations;
					NoLazyLoad = [System.Boolean]$NoLazyLoad;
					DefaultAttributes = [System.Boolean]($DefaultAttributes -and (-not $Repository)) ;
				} `
				-SuccessMessage "Added Models output at {0}" `
				-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force
		}
    
		End-Query
	}

	if (-not $DontConfig)
	{
		Invoke-Scaffolder Config $ServerName $DatabaseName `
				-Force `
				-Project $Project `
				-CodeLanguage $CodeLanguage 


	}

} catch [System.Exception] {
	Write-Error $Error[0];
}