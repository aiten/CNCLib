Param(
	[Parameter(Mandatory=$False)]
    [string]$SourceBinFolder = "..\WpfClient.WebAPI.Start\bin\Debug\netcoreapp3.1"
)

$OutputDir = "./Output"
if(!(Test-Path -Path $OutputDir )){
    New-Item -ItemType directory -Path $OutputDir
}

.\nsis\makensis.exe /DCopyFromFolder="$SourceBinFolder" CNCLib.WpfClient.WebAPI.Setup.nsi 
