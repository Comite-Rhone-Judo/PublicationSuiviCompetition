try {
	Write-Host "Mise a jour de la version ..."

	$SrcIss =  "InstallerTemplate.iss"
	$DstIss = ".\bin\Release\installer.iss" 

	# Get the string from main configuration file
	$VersionString = (Select-String ..\VersionInfo.cs -Pattern "AssemblyFileVersion").ToString()
	$startPos = $VersionString.IndexOf("(") + 2
	$Version = $VersionString.Substring($startpos, 7)


	$VersionBetaString = (Select-String ..\VersionInfo.cs -Pattern "assembly: AssemblyVersionBeta").ToString()
	$startPos = $VersionBetaString.IndexOf("(") + 1
	$endPos = $VersionBetaString.IndexOf(")") - 1
	$versionBeta = $VersionBetaString.subString($startpos, $endPos - $startPos + 1)

	$VersionFinale = if ($versionBeta -eq "0") { $Version } else { $Version + ( "-beta{0}" -f ($versionBeta.PadLeft(2, '0'))) }
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