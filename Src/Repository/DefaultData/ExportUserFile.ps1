
$ScriptDir = Split-Path $script:MyInvocation.MyCommand.Path
$myArray = @()

Get-ChildItem "$ScriptDir\..\..\WpfClient\Examples" | where {$_.Attributes -NotMatch'Directory'} | Foreach-Object {
$Bytes = Get-Content $_.FullName -Encoding Byte
$EncodedText =[Convert]::ToBase64String($Bytes)

$myObject = New-Object System.Object
$myObject | Add-Member -type NoteProperty -name UserFileId -Value 1
$myObject | Add-Member -type NoteProperty -name FileName -Value "Examples/$_.FileName"
$myObject | Add-Member -type NoteProperty -name Content -Value $EncodedText

$myArray += $myObject

}

$myArray | Export-Csv -NoTypeInformation -Delimiter ';' -path "c:\tmp\test.csv" 