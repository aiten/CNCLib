Push-Location CNCLib\CNCLib.Serial.Server
try 
{
	dotnet publish -r win-x86 -c Release --output c:\tmp\CNCLib.Serial.Server\X86\publish
}
finally 
{
	Pop-Location
}
	