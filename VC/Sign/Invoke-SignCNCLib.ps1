
# Load environment variables for Visual Studio 2015, so we can
# find the right windows sdk dir to load sn.exe

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
	
	./Invoke-Environment '"%VS140COMNTOOLS%\vsvars32.bat"'

	$snExe = join-path "$($env:WindowsSDK_ExecutablePath_x86)" "sn.exe"

	$tokenNameMapping = @{
		"6C79896E3E38DBB7" = @(.\pfx2snk.ps1 .\CNCLib.pfx -pfxPassword $pfxPassword)
	}

    Try
    {
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

		    if ( !($text -match $regex1) -and !($text -match $regex2) ) 
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

            Write-Output "Signing $($_.FullName)"
		    Set-ItemProperty "$($_.FullName)" IsReadOnly $false

            &"$snExe" -R "$assName" "$keyPath" 
	    }
    }
    Finally
    {
        $tokenNameMapping.GetEnumerator() | foreach { Remove-Item $_.Value }
    }
#}
