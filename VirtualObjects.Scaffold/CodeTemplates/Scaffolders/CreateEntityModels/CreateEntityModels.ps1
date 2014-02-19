[T4Scaffolding.Scaffolder(Description = "Enter a description of CreateEntityModels here")][CmdletBinding()]
param(       
	[parameter(Mandatory=$true, ValueFromPipelineByPropertyName = $true)][String]$ConnectionName, 
    [string]$Project,
	[string]$CodeLanguage,
	[string[]]$TemplateFolders,
	[switch]$Force = $false
)

$namespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value

$assemblyPath = (Get-Project).Properties.Item("LocalPath").Value + (Get-Project).ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value

# =============== LOADING DEPENDENCIES =========================

# Microsoft.SqlServer.Smo.dll
$smo = [Reflection.Assembly]::Load([io.file]::ReadAllBytes($assemblyPath + "Microsoft.SqlServer.Smo.dll"))

# VirtualObjects.Scaffold.dll
$virtualObjectsScaffold = [Reflection.Assembly]::Load([io.file]::ReadAllBytes($assemblyPath + "VirtualObjects.Scaffold.dll"))

[VirtualObjects.Scaffold.VirtualObjectsHelper]::GetTables() | foreach { 
	$outputPath = "Models\" + $_.Name

	Add-ProjectItemViaTemplate $outputPath -Template CreateEntityModelsTemplate `
		-Model @{ Namespace = $namespace; TableName = $_.Name; } `
		-SuccessMessage "Added CreateEntityModels output at {0}" `
		-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

}


