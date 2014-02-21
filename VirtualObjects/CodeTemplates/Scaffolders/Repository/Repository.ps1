[T4Scaffolding.Scaffolder(Description = "Enter a description of Repository here")][CmdletBinding()]
param(      
	[string]$Project,
	[string]$CodeLanguage,
	[string[]]$TemplateFolders,
	[switch]$Force = $false,
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

$outputPath = "ExampleOutput"  
$namespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value

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