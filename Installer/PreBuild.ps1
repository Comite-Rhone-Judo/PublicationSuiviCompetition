try {
    Write-Host "Mise a jour de la version ..."

    $SrcIss = "InstallerTemplate.iss"
    $DstIss = ".\bin\Release\installer.iss" 
    $PropsPath = "..\Directory.Build.props"

    # 1. Chargement et parsing natif du fichier XML
    [xml]$xml = Get-Content $PropsPath

    # 2. Extraction directe de la version principale
    $Version = $xml.Project.PropertyGroup.Version
    if ([string]::IsNullOrWhiteSpace($Version)) {
        throw "Erreur: Balise <Version> introuvable dans le fichier $PropsPath"
    }

    # 3. Extraction de la version Beta
    $versionBeta = "0"
    # On recherche dans les ItemGroup le nœud AssemblyMetadata correspondant
    $betaNode = $xml.Project.ItemGroup.AssemblyMetadata | Where-Object { $_.Include -eq 'VersionBeta' }
    
    if ($betaNode -ne $null) {
        $versionBeta = $betaNode.Value
    }

    # 4. Formatage de la version finale
    $VersionFinale = if ($versionBeta -eq "0") { $Version } else { $Version + ( "-beta{0}" -f ($versionBeta.PadLeft(2, '0'))) }
    Write-Host "Version trouvee: $VersionFinale"

    $VersionDefine = "`n#define MyAppVersion `"$VersionFinale`""

    # 5. Generation du fichier Iss avec la version de l'application
    @($VersionDefine) + (Get-Content $SrcIss) | Set-Content $DstIss

    exit 0
}
catch {
    Write-Error $_
    exit 1
}