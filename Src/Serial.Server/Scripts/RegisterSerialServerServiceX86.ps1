$login = "NT AUTHORITY\NetworkService"
$psw = ""
$scuritypsw = ConvertTo-SecureString '$psw' -AsPlainText -Force 

$mycreds = New-Object System.Management.Automation.PSCredential($login, $scuritypsw)

sc.exe delete CNCLib.Serial.Server

$CNCServerDir = (${env:ProgramFiles(x86)}, ${env:ProgramFiles} -ne $null)[0]
$CNCServerExe = "$CNCServerDir\CNCLib.Serial.Server\CNCLib.Serial.Server.exe"

New-Service -Name "CNCLib.Serial.Server" -BinaryPathName "$CNCServerExe" -StartupType Automatic -Credential $mycreds 
Start-Service "CNCLib.Serial.Server" -Verbose
