#////////////////////////////////////////////////////////
#/*
#  This file is part of CNCLib - A library for stepper motors.
#
#  Copyright (c) 2013-2017 Herbert Aitenbichler
#
#  CNCLib is free software: you can redistribute it and/or modify
#  it under the terms of the GNU General Public License as published by
#  the Free Software Foundation, either version 3 of the License, or
#  (at your option) any later version.
#
#  CNCLib is distributed in the hope that it will be useful,
#  but WITHOUT ANY WARRANTY; without even the implied warranty of
#  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
#  GNU General Public License for more details.
#  http://www.gnu.org/licenses/
#*/

<#
.SYNOPSIS

.PARAMETER root
  Path to build directory where DLLs should be signed.
#>
#function Invoke-SignCNCLibDlls() 
#{
#	[CmdletBinding()]
	param(
		[Parameter(Mandatory = $true)]
		[ValidateScript({ Test-Path "$_" -PathType 'Container' })]
		$root,
		[String]
		$pfxPassword
	)
	
#	./Invoke-Environment '"%VS150COMNTOOLS%\vsvars32.bat"'
#	./Invoke-Environment '"C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\VC\Auxiliary\Build\vsvars32.bat"'

#	$snExe = join-path "$($env:WindowsSDK_ExecutablePath_x86)" "sn.exe"
#	$signToolsExe = join-path "$($env:WindowsSdkDir)\Bin\x86" "signtool.exe"

    $snExe = "..\TFSBin\sn.exe"
    $signToolsExe = "..\TFSBin\signtool.exe"

	$tokenNameMapping = @{
		"f1dd8891e96f0824" = @(.\pfx2snk.ps1 .\CNCLib.pfx -pfxPassword $pfxPassword)
	}

    Try
    {
	echo $pfxPassword
	    $regex1 = "Public key token is ([0-9a-z]*)"
        $regex2 = "ist ([0-9a-z]*)"

	    $files = @(ls -r -include @("*.dll", "*.exe") "$root")
	    $files | ?{ $_.FullName -notmatch "\\packages\\" } | %{

		    & "$snExe" -vf "$($_.FullName)" >$nul
		    if ($lastexitcode -eq 0) 
            {
			    return
		    }

		    [string] $text = & "$snExe" -T "$($_.FullName)"
		    if ($text -match "does not represent a strongly named assembly") 
            {
    		    # ignore native dlls.
			    return
		    }
		    if ($text -match "stellt keine Assembly mit einem starken Namen dar") 
            {
			    # ignore native dlls.
			    return
		    }

		    if ( ($text -notmatch $regex1) -and ($text -notmatch $regex2) ) 
            {
			    throw "Could not find public key token"
		    }

		    $publicKey = $matches[1]
		    if (! $tokenNameMapping.ContainsKey($publicKey)) 
            {
			    Write-Warning "Cannot sign code for key with public key token: $publicKey ($($_.FullName))"
                return
		    }

		    $keyName = $tokenNameMapping[$publicKey]
		    $keyPath = "$keyName"
            $assName = $($_.FullName)

            Write-Output "Signing $assName"
		    Set-ItemProperty "$assName" IsReadOnly $false

            &"$snExe" -R "$assName" "$keyPath" 
			if ($lastexitcode -ne 0) 
			{
				throw "Could not sign $assName"
			}

            &"$signToolsExe" sign /f ".\CNCLib.pfx" /p "$pfxPassword" "$assName" 
			if ($lastexitcode -ne 0) 
			{
				throw "Could not sign $assName"
			}
		}
    }
    Finally
    {
        $tokenNameMapping.GetEnumerator() | foreach { Remove-Item $_.Value }
    }
#}
