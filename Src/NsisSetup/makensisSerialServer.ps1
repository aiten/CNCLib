Param(
	[Parameter(Mandatory=$False)]
    [string]$SourceBinFolder = "..\CNCLib\CNCLib.Serial.Server\bin\Debug\netcoreapp2.1"
)

$OutputDir = "./Output"
if(!(Test-Path -Path $OutputDir )){
    New-Item -ItemType directory -Path $OutputDir
}

.\nsis\makensis.exe /DCopyFromFolder="$SourceBinFolder" CNCLib.Serial.Server.Setup.nsi 
