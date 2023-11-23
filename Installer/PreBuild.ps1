try {
	Write-Host "Mise a jour de la version ..."

	$SrcIss =  "InstallerTemplate.iss"
	$DstIss = ".\bin\Release\installer.iss" 

	# Get the string from main configuration file
	$VersionString = (Select-String ..\VersionInfo.cs -Pattern "AssemblyFileVersion").ToString()
	$startPos = $VersionString.IndexOf("(") + 2
	$Version = $VersionString.Substring($startpos, 7)
	$VersionDefine = "`n#define MyAppVersion `"" + $Version + "`""

	# Generate the Iss file with the App version
	@($VersionDefine) + (Get-Content $SrcIss) | Set-Content $DstIss

	exit 0
}
catch {
	$_
	exit 1
}