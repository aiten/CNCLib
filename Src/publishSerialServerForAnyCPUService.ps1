Push-Location CNCLib\CNCLib.Serial.Server
try 
{
	dotnet publish -c Release --output c:\tmp\CNCLib.Serial.Server\AnyCPU\publish
}
finally 
{
	Pop-Location
}
	