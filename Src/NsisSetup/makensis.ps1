Param(
	[Parameter(Mandatory=$False)]
    [string]$SourceBinFolder = "..\CNCLib\CNCLib.Wpf.Start\bin\Debug"
)

$OutputDir = "./Output"
if(!(Test-Path -Path $OutputDir )){
    New-Item -ItemType directory -Path $OutputDir
}

.\nsis\makensis.exe /DCopyFromFolder="$SourceBinFolder" CNCLib.Wpf.Setup.nsi 
