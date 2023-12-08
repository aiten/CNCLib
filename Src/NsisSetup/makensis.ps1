Param(
	[Parameter(Mandatory=$False)]
    [string]$SourceBinFolder = "..\WpfClient.Start\bin\Debug\net8.0-windows"
)

$OutputDir = "./Output"
if(!(Test-Path -Path $OutputDir )){
    New-Item -ItemType directory -Path $OutputDir
}

.\nsis\makensis.exe /DCopyFromFolder="$SourceBinFolder" CNCLib.WpfClient.Setup.nsi 
