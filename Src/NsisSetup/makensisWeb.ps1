Param(
	[Parameter(Mandatory=$False)]
    [string]$SourceBinFolder = "..\CNCLib\WpfClient.WebAPI.Start\bin\Debug"
)

$OutputDir = "./Output"
if(!(Test-Path -Path $OutputDir )){
    New-Item -ItemType directory -Path $OutputDir
}

.\nsis\makensis.exe /DCopyFromFolder="$SourceBinFolder" CNCLib.WpfClient.WebAPI.Setup.nsi 
