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
	[System.String]$TableName = "-",
	[System.String]$ModelFolder = "Models",
	[System.String]$RepositoryFolder = "Repositories",
	[System.String]$AnnotationsFolder = "Annotations",
	[System.String]$ToFolder = "-"
)

if (-not ($ToFolder -eq "-"))
{
	$ModelFolder =  "$ToFolder\$ModelFolder"
	$RepositoryFolder =  "$ToFolder\$RepositoryFolder"
	$AnnotationsFolder =  "$ToFolder\$AnnotationsFolder"
}

$namespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value


function Load-PackageAssembly($packageName) 
{
	$assemblyPath = Get-NugetAssemblyPath($packageName)

	Write-Verbose "Assembly found at: $assemblyPath"

	$virtualObjects = [System.Reflection.Assembly]::Load([System.IO.File]::ReadAllBytes($assemblyPath + "$packageName.dll"))
}

function AppendNet45Folder($path)
{
	$net45Folders = (Get-ChildItem $path -Filter net45*)

	if ($net45Folders.Count -eq 0)
	{
		return AppendNet40Folder($path);
	}

	$net45Folder = $net45Folders[0].Name;
	return ($path + "$net45Folder\")
}

function AppendNet40Folder($path)
{
	$net40Folders = (Get-ChildItem $path -Filter net40*)
	$net40Folder = $net40Folders[0].Name;
	return ($path + "$net40Folder\")
}

function Get-NugetAssemblyPath($packageName)
{
	$package = (Get-Package -Filter $packageName)

	if (-not $package)
	{
		throw "Unable to find the package $package."
	}

	$targetFramework = [System.String](Get-Project $Project).Properties.Item("TargetFrameworkMoniker").Value
	
	$backupfolder = "..\";

	$packagesPath = (Get-Project).Properties.Item("LocalPath").Value + $backupfolder + "packages\$packageName." + (Get-Package -Filter $packageName -Skip ((Get-Package -Filter $packageName).Count-1)).Version.ToString() + "\lib\" 
	
	Write-Verbose "Searching assembly in: $packagesPath"

	while (-not [System.IO.Directory]::Exists($packagesPath)) 
	{
		$backupfolder = $backupfolder + "..\";
		$packagesPath = (Get-Project).Properties.Item("LocalPath").Value + $backupfolder + "packages\$packageName." + (Get-Package -Filter $packageName -Skip ((Get-Package -Filter $packageName).Count-1)).Version.ToString() + "\lib\" 
		Write-Verbose "Searching assembly in: $packagesPath"
	}

	if ($targetFramework.EndsWith("v4.0"))
	{
		return AppendNet40Folder($packagesPath)
	}
	else
	{
		return AppendNet45Folder($packagesPath)
	}
}


$assemblyPath = $packagesPath

# =============== LOADING DEPENDENCIES =========================

Write-Verbose "Loading Ninject"
Load-PackageAssembly('Ninject')

Write-Verbose "Loading FasterFlect"
Load-PackageAssembly('Fasterflect')

Write-Verbose "Loading Castle.Core"
Load-PackageAssembly('Castle.Core')

Write-Verbose "Loading VirtualObjects"
Load-PackageAssembly('VirtualObjects')


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
		Invoke-Scaffolder Repository -Project $Project -CodeLanguage $CodeLanguage -ModelFolder $ModelFolder -RepositoryFolder $RepositoryFolder -AnnotationsFolder $AnnotationsFolder
	}
}

[VirtualObjects.Scaffold.VirtualObjectsHelper]::GetTables($DatabaseName, $ServerName) | ForEach-Object { 
	$table = $_

	if ($TableName -eq "-" -or $TableName -eq $table.Name) {
		$outputPath = "$ModelFolder\" + (Get-SingularizedWord $table.Name)
		
		$tableDynamic = @{
			Name = $table.Name;
			NameSingularized = (Get-SingularizedWord $table.Name);
		}
		
		$tableDynamic.Columns = @()

		$table.Columns | foreach {
			$column = $_
			
			$columnDynamic = @{
				Name = $column.Name;
				NameSingularized = $column.Name;
				Identity = $column.Identity;
				InPrimaryKey = $column.InPrimaryKey;
				IsForeignKey = $column.IsForeignKey;
				DataType = $column.DataType;
			}

			$columnDynamic.ForeignKeys = @()

			$column.ForeignKeys | foreach {
				$foreignKey = $_

				$columnDynamic.ForeignKeys += @{
					ReferencedTableName = (Get-SingularizedWord $foreignKey.ReferencedTable.Name);
					ReferencedColumnName = $foreignKey.ReferencedColumn.Name;
					TableName = (Get-SingularizedWord $foreignKey.Table.Name);
					ColumnName = $foreignKey.Column.Name;
				}
			}
			
			$tableDynamic.Columns += $columnDynamic
		}

		Add-ProjectItemViaTemplate $outputPath -Template EntityTemplate `
			-Model @{ 
				Namespace = $namespace; 
				ServerName = $ServerName; 
				DatabaseName = $DatabaseName; 
				Table = $tableDynamic 
				Path = $assemblyPath;
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
}

if (-not $DontConfig)
{
	Invoke-Scaffolder Config $ServerName $DatabaseName `
			-Force `
			-Project $Project `
			-CodeLanguage $CodeLanguage 


}