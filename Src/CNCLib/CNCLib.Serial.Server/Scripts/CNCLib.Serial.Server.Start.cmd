start "CNCLib.Serial.Server" dotnet "%~dp0\CNCLib.Serial.Server.dll" --urls "http://*:5000"
start "CNCLib.Serial.Server Web-Interface"  http://localhost:5000