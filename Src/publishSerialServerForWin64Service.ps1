Push-Location CNCLib\CNCLib.Serial.Server
try {
	dotnet publish -r win-x64 -c Release
}
finally {
	Pop-Location
}
