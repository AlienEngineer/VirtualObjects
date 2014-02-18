[T4Scaffolding.Scaffolder(Description = "Enter a description of CreateEntityModels here")][CmdletBinding()]
param(       
	[parameter(Mandatory = $true, ValueFromPipelineByPropertyName = $true)][String]$TableName, 
    [string]$Project,
	[string]$CodeLanguage,
	[string[]]$TemplateFolders,
	[switch]$Force = $false
)

$outputPath = "Models\" + $TableName  # The filename extension will be added based on the template's <#@ Output Extension="..." #> directive
$namespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value

Add-ProjectItemViaTemplate $outputPath -Template CreateEntityModelsTemplate `
	-Model @{ Namespace = $namespace; TableName = $TableName; } `
	-SuccessMessage "Added CreateEntityModels output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force
