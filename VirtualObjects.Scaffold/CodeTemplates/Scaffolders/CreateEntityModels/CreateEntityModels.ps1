[T4Scaffolding.Scaffolder(Description = "Enter a description of CreateEntityModels here")][CmdletBinding()]
param(       
	[parameter(Mandatory=$true, ValueFromPipelineByPropertyName = $true)][String]$ServerName, 
	[parameter(Mandatory=$true, ValueFromPipelineByPropertyName = $true)][String]$DatabaseName, 
    [string]$Project,
	[string]$CodeLanguage,
	[string[]]$TemplateFolders,
	[switch]$Force = $false,
	[switch]$Repository = $false,
	[string]$TableName = "-"
)



$namespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value

$assemblyPath = (Get-Project).Properties.Item("LocalPath").Value + (Get-Project).ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value

# =============== LOADING DEPENDENCIES =========================

# Microsoft.SqlServer.Smo.dll
$smo = [Reflection.Assembly]::Load([io.file]::ReadAllBytes($assemblyPath + "Microsoft.SqlServer.Smo.dll"))

# VirtualObjects.Scaffold.dll
$virtualObjectsScaffold = [Reflection.Assembly]::Load([io.file]::ReadAllBytes($assemblyPath + "VirtualObjects.Scaffold.dll"))


if($Repository) {

	Write-Verbose " -> Creation Repository Layer started."
	Write-Verbose "==============================================================="
	
	$outputPath = "Repositories\IRepository";

	Add-ProjectItemViaTemplate $outputPath -Template IRepositoryTemplate `
		-Model @{ Namespace = $namespace; } `
		-SuccessMessage "Added IRepositoryTemplate output at {0}" `
		-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force
	
	$outputPath = "Repositories\Repository";

	Add-ProjectItemViaTemplate $outputPath -Template RepositoryTemplate `
		-Model @{ Namespace = $namespace; } `
		-SuccessMessage "Added RepositoryTemplate output at {0}" `
		-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force
	
	Write-Verbose " -> Repository Layer creation ended."
	Write-Verbose "==============================================================="
}

$outputPath = "Annotations\Annotations";

Add-ProjectItemViaTemplate $outputPath -Template AnnotationsTemplate `
	-Model @{ Namespace = $namespace; } `
	-SuccessMessage "Added AnnotationTemplate output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force


[VirtualObjects.Scaffold.VirtualObjectsHelper]::GetTables($DatabaseName, $ServerName) | foreach { 
	if ($TableName -eq "-" -or $TableName -eq $_.Name) {
		$outputPath = "Models\" + $_.Name

		Add-ProjectItemViaTemplate $outputPath -Template CreateEntityModelsTemplate `
			-Model @{ 
				Namespace = $namespace; 
				ServerName = $ServerName; 
				DatabaseName = $DatabaseName; 
				TableId = $_.Id; 
				TableName = $_.Name; 
			} `
			-SuccessMessage "Added CreateEntityModels output at {0}" `
			-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

	}
}


