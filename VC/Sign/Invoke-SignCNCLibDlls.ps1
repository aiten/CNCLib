
# Load environment variables for Visual Studio 2015, so we can
# find the right windows sdk dir to load sn.exe


<#
.SYNOPSIS

.PARAMETER root
  Path to build directory where DLLs should be signed.
#>
function Invoke-SignCNCLibDlls() {
	[CmdletBinding()]
	param(
		[Parameter(Mandatory = $true)]
		[ValidateScript({ Test-Path "$_" -PathType 'Container' })]
		$root
	)
	
	./Invoke-Environment '"%VS140COMNTOOLS%\vsvars32.bat"'

	$snExe = join-path "$($env:WindowsSDK_ExecutablePath_x86)" "sn.exe"
	$baseKeyPath = "c:\user\Herbert\Arduino\Stepper\VC\Sign\"

	$tokenNameMapping = @{
		"6C79896E3E38DBB7" = "CNCLib.pfx"
	}

    $regex = "ist ([0-9a-z]*)"

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


		if ( !($text -match $regex) ) {
			throw "Could not find public key token"
		}

		$publicKey = $matches[1]
		if (! $tokenNameMapping.ContainsKey($publicKey)) 
        {
			Write-Warning "Cannot sign code for key with public key token: $publicKey ($($_.FullName))"
            return
		}

		$keyName = $tokenNameMapping[$publicKey]
		$keyPath = join-path "$baseKeyPath" "$keyName"
        $assName = $($_.FullName)

        Write-Output "Signing $($_.FullName)"
		Set-ItemProperty "$($_.FullName)" IsReadOnly $false

        echo "CNC-Lib1" | &"$snExe" -R "$assName" "$keyPath" 
        #Get-Content C:\tmp\keyfile.txt | &"$snExe" -R "$assName" "$keyPath" 
        #Invoke-SN $snExe $assName $keyPath
	}
}

function Invoke-SN() {
	[CmdletBinding()]
	param(
		[Parameter(Mandatory = $true)]
        [String]
        $snExe,
        [String]
        $assName,
        [String]
        $keyPath
	)


        &"$snExe" -R "$assName" "$keyPath"
              
		if ($lastexitcode -ne 0) {
			throw "Could not sign $assName"
		}
}