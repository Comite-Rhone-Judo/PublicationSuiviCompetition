try {
	Write-Host "Mise a jour de la version ..."

	$SrcIss =  "InstallerTemplate.iss"
	$DstIss = ".\bin\Release\installer.iss" 

	# Get the string from main configuration file
	$VersionString = (Select-String ..\VersionInfo.cs -Pattern "AssemblyFileVersion").ToString()
	$startPos = $VersionString.IndexOf("(") + 2
	$Version = $VersionString.Substring($startpos, 7)


	$VersionTestString = (Select-String ..\VersionInfo.cs -Pattern "assembly: AssemblyVersionTest").ToString()
	$startPos = $VersionTestString.IndexOf("(") + 1
	$endPos = $VersionTestString.IndexOf(")") - 1
	$versionTest = $VersionTestString.subString($startpos, $endPos - $startPos + 1)

	$VersionFinale = if ($versionTest -eq "0") { $Version } else { $Version + "_test" + $versionTest }
	Write-Host "Version trouvee: "  $VersionFinale

	$VersionDefine = "`n#define MyAppVersion `"" + $VersionFinale + "`""

	# Generate the Iss file with the App version
	@($VersionDefine) + (Get-Content $SrcIss) | Set-Content $DstIss

	exit 0
}
catch {
	$_
	exit 1
}