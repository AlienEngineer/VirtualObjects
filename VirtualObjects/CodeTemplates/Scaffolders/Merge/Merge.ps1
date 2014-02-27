<#
.SYNOPSIS
IlMerges an assembly with its dependencies. Depends on nuget being installed in the PATH.
 
.PARAMETER targetProject
The name of the project to be ilmerged
 
.PARAMETER outputAssembly
The name of the ilmerged assembly when it is created
 
.PARAMETER buildConfiguration
The build configuration used to create the assembly. Used to locate the assembly under the project. The usual format is Project/bin/Debug
 
.PARAMETER targetPlatform
Defaults to .NET 4
 
.PARAMETER mvc3
If the project is an Mvc3 project, the MVC3 assemblies need to be added to the ilmerge list. The script assumes that the MVC3 assemblies are installed in the default location.
 
.PARAMETER internalize
Adds the /internalize flag to the merged assembly to prevent namespace conflicts.
 
.EXAMPLE
ilmerge.ps1 -targetProject "MyMVC3Project" -mvc3
 
#>
[T4Scaffolding.Scaffolder(Description = "Enter a description of Merge here")][CmdletBinding()]
param(
	[parameter(Mandatory=$true, ValueFromPipelineByPropertyName = $true)][System.String]$targetVersion, 
    $buildConfiguration = "",
    [switch] $internalize
)

 
function Get-InputAssemblyNames($buildDirectory)
{
    $assemblyNames = Get-ChildItem -Path $buildDirectory -Filter *.dll | ForEach-Object { """" + $_.FullName + """" }
	write-host "Assemblies to merge: $assemblyNames"
 
    $inArgument = [System.String]::Join(" ", $assemblyNames)
    return $inArgument
}
 
function Get-BuildDirectory()
{
	return (Get-Project).Properties.Item("LocalPath").Value + "bin\Release\$targetVersion\"
}
 
try
{
	$targetProject = (Get-Project).Name
	$outputAssembly = "$targetProject.dll" 
	$buildDirectory = Get-BuildDirectory

	$solutionDirectoryFullName = (Get-Project).Properties.Item("LocalPath").Value + "..\"
	$ilMergeAssembly = "$solutionDirectoryFullName\.ilmerge\IlMerge\IlMerge.exe"
	$publishDirectory = "$buildDirectory..\Publish\$targetVersion"
	$outputAssemblyFullPath = "$publishDirectory\$outputAssembly"
 
	
 
	"Script Directory  : $scriptPath"
	"Solution Directory: $solutionDirectoryFullName"
	"Build Directory   : $buildDirectory"
	"Publish Directory : $publishDirectory"
 
 
	$outArgument = "/out:$publishDirectory/$outputAssembly"
	$inArgument = Get-InputAssemblyNames $buildDirectory
 
 	#    They have to be referenced directly.
 
	$cmd = "$ilMergeAssembly /t:library /ndebug /xmldocs /attr:$buildDirectory$targetProject.dll $outArgument $inArgument"
 
	if ($internalize)
	{
		$cmd = $cmd + " /internalize"
	} 
	
	Write-Host $cmd

	"Installing ilmerge"
	nuget install IlMerge -outputDirectory .ilmerge -ExcludeVersion
 
	"Ensuring that publication directory exists"
	if ([System.IO.Directory]::Exists($publishDirectory) -eq $false)
	{
		[System.IO.Directory]::CreateDirectory($publishDirectory)
	}
 
	"Running Command: $cmd"
	$result = Invoke-Expression $cmd
 
	"Getting assembly info for $outputAssemblyFullPath"
 
	$outputAssemblyInfo = New-Object System.IO.FileInfo $outputAssemblyFullPath
	if ($outputAssemblyInfo.Length -eq 0)
	{
		$outputAssemblyInfo.Delete	
	}
 
	$outputAssemblyInfo
 
	if ($outputAssemblyInfo.Exists -eq $false)
	{
		throw "Output assembly not created by ilmerge script."
		Exit -1
	}
	elseif ($outputAssemblyInfo.Length -eq 0) 
	{
		$outputAssemblyInfo.Delete();
		Exit -1;
	}
	else
	{
		"Output assembly created successfully at $outputAssemblyFullPath."
		Exit 0;
	}
}
catch
{
	throw
	Exit -1
}