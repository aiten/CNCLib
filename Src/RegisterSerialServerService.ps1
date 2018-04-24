$login = "NT AUTHORITY\NetworkService"
$psw = ""
$scuritypsw = ConvertTo-SecureString '$psw' -AsPlainText -Force 

$mycreds = New-Object System.Management.Automation.PSCredential($login, $scuritypsw)

sc.exe delete CNCLib.SerialServer

New-Service -Name "CNCLib.SerialServer" -BinaryPathName '"dotnet" "C:\Program Files (x86)\CNCLib.Serial.Server\CNCLib.Serial.Server.dll"' -StartupType Automatic -Credential $mycreds 
Start-Service "CNCLib.SerialServer" -Verbose
pause