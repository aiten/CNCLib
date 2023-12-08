Param(
	[Parameter(Mandatory=$False)]
    [string]$SourceBinFolder = "..\Serial.Server\bin\Debug\net8.0"
)

$OutputDir = "./Output"
if(!(Test-Path -Path $OutputDir )){
    New-Item -ItemType directory -Path $OutputDir
}

.\nsis\makensis.exe /DCopyFromFolder="$SourceBinFolder" CNCLib.Serial.Server.Setup.nsi 
