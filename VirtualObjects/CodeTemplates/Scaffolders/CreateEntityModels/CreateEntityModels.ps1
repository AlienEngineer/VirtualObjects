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

$assemblyPath = (Get-Project).Properties.Item("LocalPath").Value + (Get-Project).ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value

# =============== LOADING DEPENDENCIES =========================

# Microsoft.SqlServer.Smo.dll
$smo = [Reflection.Assembly]::Load([io.file]::ReadAllBytes($assemblyPath + "Microsoft.SqlServer.Smo.dll"))

# VirtualObjects.Scaffold.dll
$virtualObjects = [Reflection.Assembly]::Load([io.file]::ReadAllBytes($assemblyPath + "VirtualObjects.dll"))


if($Repository) {

	Write-Verbose " -> Creation Repository Layer started."
	Write-Verbose "==============================================================="
	
	$outputPath = "$RepositoryFolder\IRepository";

	Add-ProjectItemViaTemplate $outputPath -Template IRepositoryTemplate `
		-Model @{ 
			Namespace = $namespace; 
			AnnotationsFolder = $AnnotationsFolder; 
			RepositoryFolder = $RepositoryFolder;
			ModelFolder = $ModelFolder;
		} `
		-SuccessMessage "Added IRepositoryTemplate output at {0}" `
		-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force
	
	$outputPath = "$RepositoryFolder\Repository";

	Add-ProjectItemViaTemplate $outputPath -Template RepositoryTemplate `
		-Model @{ 
			Namespace = $namespace; 
			AnnotationsFolder = $AnnotationsFolder; 
			RepositoryFolder = $RepositoryFolder;
			ModelFolder = $ModelFolder;
		} `
		-SuccessMessage "Added RepositoryTemplate output at {0}" `
		-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

	$outputPath = "$RepositoryFolder\RepositoryExtensions";

	Add-ProjectItemViaTemplate $outputPath -Template RepositoryExtensionsTemplate `
		-Model @{ 
			Namespace = $namespace; 
			AnnotationsFolder = $AnnotationsFolder; 
			RepositoryFolder = $RepositoryFolder;
			ModelFolder = $ModelFolder;
		} `
		-SuccessMessage "Added RepositoryExtensionsTemplate output at {0}" `
		-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force
	
	Write-Verbose " -> Repository Layer creation ended."
	Write-Verbose "==============================================================="

	if (-not $UsingCustomAnnotations)
	{
		$outputPath = "$AnnotationsFolder\Annotations";

		Add-ProjectItemViaTemplate $outputPath -Template AnnotationsTemplate `
			-Model @{ 
				Namespace = $namespace; 
				AnnotationsFolder = $AnnotationsFolder; 
				RepositoryFolder = $RepositoryFolder;
				ModelFolder = $ModelFolder;
			} `
			-SuccessMessage "Added AnnotationTemplate output at {0}" `
			-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force
	}
}

[VirtualObjects.Scaffold.VirtualObjectsHelper]::GetTables($DatabaseName, $ServerName) | foreach { 
	if ($TableName -eq "-" -or $TableName -eq $_.Name) {
		$outputPath = "$ModelFolder\" + $_.Name

		Add-ProjectItemViaTemplate $outputPath -Template CreateEntityModelsTemplate `
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
			} `
			-SuccessMessage "Added CreateEntityModels output at {0}" `
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