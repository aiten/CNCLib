Push-Location CNCLib\CNCLib.WebAPI
try 
{
	dotnet publish -r win-x64 -c Release --output c:\tmp\CNCLib.WebAPI\X64\publish
}
finally 
{
	Pop-Location
}
