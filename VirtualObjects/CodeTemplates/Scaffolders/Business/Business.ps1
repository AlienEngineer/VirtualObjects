[T4Scaffolding.Scaffolder(Description = "Creates a business layer")][CmdletBinding()]
param(        
	[parameter(Mandatory=$true, ValueFromPipelineByPropertyName = $true)][System.String]$Business, 
    [string]$Project,
	[string]$CodeLanguage,
	[string[]]$TemplateFolders,
	[switch]$Force = $false,
	[string]$BusinessFolder = "Business",
	[string]$ModelFolder = "Models",
	[string]$ToFolder = "-",
	[switch]$NoDelete = $false,
	[switch]$NoInsert = $false,
	[switch]$NoUpdate = $false,
	[switch]$ReadOnly = $false
)

if ($ReadOnly) 
{
	$NoDelete = $ReadOnly
	$NoUpdate = $ReadOnly 
	$NoInsert = $ReadOnly 
}

if (-not ($ToFolder -eq "-"))
{
	$BusinessFolder =  "$ToFolder\$BusinessFolder"
}

$namespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value

$outputPath = "$BusinessFolder\I" + (Get-PluralizedWord $Business)

Write-Host ("Creating business module for: " + (Get-SingularizedWord $Business))

Add-ProjectItemViaTemplate $outputPath -Template EntityBusinessInterfaceTemplate `
	-Model @{ 
		Namespace = $namespace; 
		BusinessFolder = $BusinessFolder;  
		Business = (Get-SingularizedWord $Business);
		BusinessPluralized = (Get-PluralizedWord $Business);
		ModelFolder = $ModelFolder;
	} `
	-SuccessMessage "Added Business output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

$outputPath = "$BusinessFolder\" + (Get-PluralizedWord $Business)

Add-ProjectItemViaTemplate $outputPath -Template EntityBusinessTemplate `
	-Model @{ 
		Namespace = $namespace; 
		BusinessFolder = $BusinessFolder;  
		Business = (Get-SingularizedWord $Business);
		BusinessPluralized = (Get-PluralizedWord $Business);
		ModelFolder = $ModelFolder;
		NoDelete = [bool]$NoDelete;
		NoInsert = [bool]$NoInsert;
		NoUpdate = [bool]$NoUpdate;
	} `
	-SuccessMessage "Added Business output at {0}" `
	-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force

## Appends the Business Layer base.
#$outputPath = "$BusinessFolder\Business" 

#if ((Get-ProjectItem ($outputPath + ".cs")) -eq $null)
#{
#	Add-ProjectItemViaTemplate $outputPath -Template BusinessTemplate `
#		-Model @{ Namespace = $namespace; BusinessFolder = $BusinessFolder;  } `
#		-SuccessMessage "Added Business output at {0}" `
#		-TemplateFolders $TemplateFolders -Project $Project -CodeLanguage $CodeLanguage -Force:$Force
#}