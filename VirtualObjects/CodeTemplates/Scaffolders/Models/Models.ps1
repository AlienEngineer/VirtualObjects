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

# Getting the package path for the proper version of VirtualObjects.
$targetFramework = [System.String](Get-Project $Project).Properties.Item("TargetFrameworkMoniker").Value

$backupfolder = "..\";

$packagesPath = (Get-Project).Properties.Item("LocalPath").Value + $backupfolder + "packages\VirtualObjects." + (Get-Package -Filter VirtualObjects -Skip ((Get-Package -Filter VirtualObjects).Count-1)).Version.ToString() + "\lib\" 


while (-not [System.IO.Directory]::Exists($packagesPath)) 
{
	$backupfolder = $backupfolder + "..\";
	$packagesPath = (Get-Project).Properties.Item("LocalPath").Value + $backupfolder + "packages\VirtualObjects." + (Get-Package -Filter VirtualObjects -Skip ((Get-Package -Filter VirtualObjects).Count-1)).Version.ToString() + "\lib\" 
}

if ($targetFramework.EndsWith("v4.0"))
{
	$packagesPath = $packagesPath + "net40\"
}
else
{
	$packagesPath = $packagesPath + "net45\"
}

#$assemblyPath = (Get-Project).Properties.Item("LocalPath").Value + (Get-Project).ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value
$assemblyPath = $packagesPath

# =============== LOADING DEPENDENCIES =========================

$ninject = [System.Reflection.Assembly]::Load([System.IO.File]::ReadAllBytes($assemblyPath + "Ninject.dll"))
$castleCore = [System.Reflection.Assembly]::Load([System.IO.File]::ReadAllBytes($assemblyPath + "Castle.Core.dll"))
$fasterflact = [System.Reflection.Assembly]::Load([System.IO.File]::ReadAllBytes($assemblyPath + "Fasterflect.dll"))

# VirtualObjects.Scaffold.dll
$virtualObjects = [System.Reflection.Assembly]::Load([System.IO.File]::ReadAllBytes($assemblyPath + "VirtualObjects.dll"))


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

		Write-Host ("Name			: " + $tableDynamic.Name)
		Write-Host ("Singularized	: " + $tableDynamic.NameSingularized)
		Write-Host ("Columns.Count	: " + $table.Columns.Count)
		
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
				}
			}
			
			$tableDynamic.Columns += $columnDynamic
			Write-Host ("	DataType			: " + $columnDynamic.DataType)
		}

		$TableName = $table.Name

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