[T4Scaffolding.Scaffolder(Description = "Enter a description of CreatePackage here")][CmdletBinding()]
param(        
	[parameter(Mandatory=$true, ValueFromPipelineByPropertyName = $true)][System.String]$version,
    [string]$Project,
	[string]$CodeLanguage,
	[string[]]$TemplateFolders,
	[switch]$Force = $false,
	[switch]$Push = $false
)

$namespace = (Get-Project $Project).Properties.Item("DefaultNamespace").Value

Write-Host "Packing version $version...."
Write-Host ""
Invoke-Scaffolder Merge net40
Write-Host ""
Invoke-Scaffolder Merge net45
Write-Host ""

nuget pack .\VirtualObjects\VirtualObjects.csproj -Symbols -Version $version
Write-Host "Packing done version $version"

if ($push) 
{
	nuget push "VirtualObjects.$version.nupkg"
}
else 
{
	Write-Host "Version $version not yet published."
}