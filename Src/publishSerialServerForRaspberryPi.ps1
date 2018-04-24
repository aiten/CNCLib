Push-Location CNCLib\CNCLib.Serial.Server
try 
{
	dotnet publish -r linux-arm -c Release --output c:\tmp\CNCLib.Serial.Server\RasperyPi\publish
}
finally 
{
	Pop-Location
}
	