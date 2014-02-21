[T4Scaffolding.Scaffolder(Description = "Creation of entity models, and a repository layer.")][CmdletBinding()]
param(       
	[parameter(Mandatory=$true, ValueFromPipelineByPropertyName = $true)][String]$ServerName, 
	[parameter(Mandatory=$true, ValueFromPipelineByPropertyName = $true)][String]$DatabaseName, 
    [string]$Project,
	[string]$CodeLanguage,
	[string[]]$TemplateFolders,
	[switch]$Force = $false,
	[switch]$Repository = $false,
	[switch]$NoLazyLoad = $false,
	[switch]$WithAnnotations = $false,
	[switch]$DontConfig = $false,
	[switch]$UsingCustomAnnotations = $false,
	[switch]$DefaultAttributes = $false,
	[string]$TableName = "-",
	[string]$ModelFolder = "Models",
	[string]$RepositoryFolder = "Repositories",
	[string]$AnnotationsFolder = "Annotations",
	[String]$ToFolder = "-"
)

if (-not ($ToFolder -eq "-"))
{
	$ModelFolder =  "$ToFolder\$ModelFolder"
	$RepositoryFolder =  "$ToFolder\$RepositoryFolder"
	$AnnotationsFolder =  "$ToFolder\$AnnotationsFolder"
}

$namespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value

# Getting the package path for the proper version of VirtualObjects.
$targetFramework = [string](Get-Project $Project).Properties.Item("TargetFrameworkMoniker").Value

$packagesPath = (Get-Project).Properties.Item("LocalPath").Value + "..\packages\VirtualObjects." + (Get-Package -Filter VirtualObjects -Skip ((Get-Package -Filter VirtualObjects).Count-1)).Version.ToString() + "\lib\" 

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

# Microsoft.SqlServer.Smo.dll
$smo = [Reflection.Assembly]::Load([io.file]::ReadAllBytes($assemblyPath + "Microsoft.SqlServer.Smo.dll"))

# VirtualObjects.Scaffold.dll
$virtualObjects = [Reflection.Assembly]::Load([io.file]::ReadAllBytes($assemblyPath + "VirtualObjects.dll"))


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

[VirtualObjects.Scaffold.VirtualObjectsHelper]::GetTables($DatabaseName, $ServerName) | foreach { 
	if ($TableName -eq "-" -or $TableName -eq $_.Name) {
		$outputPath = "$ModelFolder\" + $_.Name

		Add-ProjectItemViaTemplate $outputPath -Template ModelsTemplate `
			-Model @{ 
				Namespace = $namespace; 
				ServerName = $ServerName; 
				DatabaseName = $DatabaseName; 
				TableId = $_.Id; 
				TableName = $_.Name; 
				AnnotationsFolder = $AnnotationsFolder; 
				RepositoryFolder = $RepositoryFolder;
				ModelFolder = $ModelFolder;
				ForceAnnotations = [Boolean]$WithAnnotations;
				NoLazyLoad = [Boolean]$NoLazyLoad;
				DefaultAttributes = [Boolean]($DefaultAttributes -and (-not $Repository)) ;
			} `
			-SuccessMessage "Added Models output at {0}" `
			-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

	}
}

if (-not $DontConfig)
{
	# get the full path and file name of the App.config file in the same directory as this script
	$appConfigFile = [IO.Path]::Combine((Get-Project).Properties.Item("LocalPath").Value, 'App.config')

	if(![System.IO.File]::Exists($appConfigFile)) {
		$appConfigFile = [IO.Path]::Combine((Get-Project).Properties.Item("LocalPath").Value, 'Web.config')
	}

	if([System.IO.File]::Exists($appConfigFile)) {
		$appConfig = New-Object XML
		$appConfig.Load($appConfigFile)

		foreach($connectionString in $appConfig.configuration.connectionStrings.add)
		{
			$connectionString.providerName ="System.Data.SqlClient"
			$connectionString.connectionString = "Data Source=$ServerName;Initial Catalog=$DatabaseName;Integrated Security=True"
		}

		$appConfig.Save($appConfigFile)	
	}
	else 
	{
		Write-Host ""
		Write-Host "Unable to find any configuration file to change."
		Write-Host ""
	}
}