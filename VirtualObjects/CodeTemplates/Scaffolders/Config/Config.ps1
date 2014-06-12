[T4Scaffolding.Scaffolder(Description = "Enter a description of Config here")][CmdletBinding()]
param(        
	[parameter(Mandatory=$true, ValueFromPipelineByPropertyName = $true)][String]$ServerName, 
	[parameter(Mandatory=$true, ValueFromPipelineByPropertyName = $true)][String]$DatabaseName, 
    [string]$Project,
	[string]$CodeLanguage,
	[string[]]$TemplateFolders,
	[switch]$Force = $false,
	[string]$ConfigName = "-",
	[string]$ProviderName = "System.Data.SqlClient"
)

# get the full path and file name of the App.config file in the same directory as this script
$appConfigFile = [IO.Path]::Combine((Get-Project).Properties.Item("LocalPath").Value, 'App.config')

if(![System.IO.File]::Exists($appConfigFile)) {
	$appConfigFile = [IO.Path]::Combine((Get-Project).Properties.Item("LocalPath").Value, 'Web.config')
}

if([System.IO.File]::Exists($appConfigFile)) {
	$appConfig = New-Object XML
	$appConfig.Load($appConfigFile)

	foreach ($_ in $appConfig.configuration.ConnectionStrings.add) { 
		if ($ConfigName -eq "-" -or $ConfigName -eq $_.Name)
		{
			$_.providerName = $ProviderName
			$_.connectionString = "Data Source=$ServerName;Initial Catalog=$DatabaseName;Integrated Security=True"
		}
	}

	$appConfig.Save($appConfigFile)	
}
else 
{
	Write-Host ""
	Write-Host "Unable to find any configuration file to change."
	Write-Host ""
}