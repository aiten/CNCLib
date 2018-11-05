Param(
	[Parameter(Mandatory=$False)]
    [string]$SourceBinFolder = "..\CNCLib\CNCLib.Wpf.Start\bin\Debug"
)

.\nsis\makensis.exe /DCopyFromFolder="$SourceBinFolder" CNCLib.Wpf.Setup.nsi 
