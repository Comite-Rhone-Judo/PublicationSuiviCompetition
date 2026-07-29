<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:import href="FranceJudo.Metier/Resources/Site/xslt/entete.xslt"/>

	<xsl:output method="html" indent="yes" />
	<xsl:param name="style"/>
	<xsl:param name="js"/>
	<xsl:param name="RefData" />
	<xsl:param name="SiteRoutes" />

	<xsl:variable name="imgPath" select="$SiteRoutes/*/@urlImg"/>
	<xsl:variable name="jsPath" select="$SiteRoutes/*/@urlJs"/>
	<xsl:variable name="cssPath" select="$SiteRoutes/*/@urlCss"/>
	<xsl:variable name="commonPath" select="$SiteRoutes/*/@UrlCommon"/>

	<xsl:key name="combats" match="combat" use="@niveau"/>

	<xsl:variable select="/docroot/SiteConfiguration/@PublierProchainsCombats = 'true'" name="affProchainCombats"/>
	<xsl:variable select="/docroot/SiteConfiguration/@PublierAffectationTapis = 'true'" name="affAffectationTapis"/>
	<xsl:variable select="/docroot/SiteConfiguration/@PublierEngagements = 'true'" name="affEngagements"/>
	<xsl:variable select="/docroot/SiteConfiguration/@PublierStatistiques = 'true'" name="affStatistiques"/>
	<xsl:variable select="/docroot/SiteConfiguration/@DelaiActualisationClientSec" name="delayActualisationClient"/>
	<xsl:variable select="/docroot/SiteConfiguration/@ActualisationClientDefaut = 'true'" name="actualisationClientDefaut"/>
	<xsl:variable select="/docroot/SiteConfiguration/@Logo" name="logo"/>
	<!-- NOUVEAU : Récupération du logo sombre -->
	<xsl:variable select="/docroot/SiteConfiguration/@LogoDark" name="logoDark"/>

	<xsl:variable name="typeCompetition" select="/docroot/competition/@type"/>
	<xsl:variable name="niveauCompetition" select="/docroot/competition/@niveau"/>

	<xsl:template match="docroot">
		<xsl:text disable-output-escaping='yes'>&lt;!DOCTYPE html&gt;</xsl:text>
		<html>
			<head>
				<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
				<meta name="viewport" content="width=device-width,initial-scale=1"/>
				<meta http-equiv="Cache-Control" content="no-cache, no-store, must-revalidate"/>
				<meta http-equiv="Pragma" content="no-cache"/>
				<meta http-equiv="Expires" content="0"/>

				<link type="text/css" rel="stylesheet">
					<xsl:attribute name="href">
						<xsl:value-of select="concat($cssPath, 'w3.css')"/>
					</xsl:attribute>
				</link>
				<link type="text/css" rel="stylesheet">
					<xsl:attribute name="href">
						<xsl:value-of select="concat($cssPath, 'style-common.css')"/>
					</xsl:attribute>
				</link>
				<link type="text/css" rel="stylesheet">
					<xsl:attribute name="href">
						<xsl:value-of select="concat($cssPath, 'style-classement.css')"/>
					</xsl:attribute>
				</link>

				<script type="text/javascript">
					<xsl:value-of select="$js"/>
					gDelayAutoReloadSec = <xsl:value-of select="$delayActualisationClient"/>;
					gDefaultAutoReload = <xsl:value-of select="$actualisationClientDefaut"/>;
				</script>

				<script>
					<xsl:attribute name="src">
						<xsl:value-of select="concat($jsPath, 'site-display.js')"/>
					</xsl:attribute>
				</script>

				<title>Suivi Compétition - Classement</title>
			</head>
			<body>
				<!-- ENTETE -->
				<xsl:call-template name="entete">
					<xsl:with-param name="logo" select="$logo"/>
					<xsl:with-param name="logoDark" select="$logoDark"/>
					<xsl:with-param name="affProchainCombats" select="$affProchainCombats"/>
					<xsl:with-param name="affAffectationTapis" select="$affAffectationTapis"/>
					<xsl:with-param name="affEngagements" select="$affEngagements"/>
					<xsl:with-param name="affStatistiques" select="$affStatistiques"/>
					<xsl:with-param name="affActualiser" select="true()"/>
					<xsl:with-param name="selectedItem" select="'classement'"/>
					<xsl:with-param name="SiteRoutes" select="$SiteRoutes"/>
				</xsl:call-template>

				<!-- Nom de la competition + Catégorie (Structure Standardisée iOS) -->
				<div class="tas-competition-bandeau">
					<h4>
						<xsl:value-of select="competition/titre"/>
					</h4>
					<h5>
						<xsl:if test="//epreuve[1]/@sexe='F'">Féminines&nbsp;</xsl:if>
						<xsl:if test="//epreuve[1]/@sexe='M'">Masculins&nbsp;</xsl:if>
						<xsl:if test="//epreuve[1]/@sexe='X'">Mixte&nbsp;</xsl:if>
						<xsl:value-of select="//epreuve[1]/@nom"/>
					</h5>
				</div>

				<!-- Le classement (Carte iOS avec débordement géré) -->
				<div class="ios-card">
					<div style="overflow-x: auto; -webkit-overflow-scrolling: touch;">
						<table class="w3-table-all">
							<thead>
								<tr>
									<th>#</th>
									<xsl:choose>
										<xsl:when test="$typeCompetition = '1'">
											<th>NOM</th>
										</xsl:when>
										<xsl:otherwise>
											<th>NOM et Prénom</th>
										</xsl:otherwise>
									</xsl:choose>
									<th>Club</th>
									<xsl:if test="$niveauCompetition = '3' or $niveauCompetition = '4'">
										<th>Comité</th>
									</xsl:if>
									<xsl:if test="$niveauCompetition = '4'">
										<th>Ligue</th>
									</xsl:if>
									<xsl:if test="$niveauCompetition = '5' or $niveauCompetition = '6'">
										<th>Pays</th>
									</xsl:if>
								</tr>
							</thead>
							<tbody>
								<xsl:apply-templates select="//classement/participant">
									<xsl:sort select="@classementFinal" data-type="number" order="ascending"/>
								</xsl:apply-templates>
							</tbody>
						</table>
					</div>
				</div>

				<div class="w3-container w3-center w3-tiny text-muted tas-footnote">
					<script>
						<xsl:attribute name="src">
							<xsl:value-of select="concat($jsPath, 'footer_script.js')"/>
						</xsl:attribute>
					</script>
				</div>
			</body>
		</html>
	</xsl:template>

	<!-- TEMPLATES -->
	<!-- Ligne de classement -->
	<xsl:template match="participant">
		<xsl:variable name="participant1" select="@judoka" />
		<xsl:variable name="j1" select="//participants/participant[@judoka=$participant1]/descendant::*[1]" />

		<xsl:variable name="club" select="$RefData/structures/clubs/club[@ID=$j1/@club]"/>
		<xsl:variable name="comite" select="$RefData/structures/comites/comite[@ID=$club/@comite]"/>
		<xsl:variable name="ligue" select="$RefData/structures/ligues/ligue[@ID=$club/@ligue]"/>
		<xsl:variable name="pays" select="$RefData/structures/lesPays/pays[@ID=$j1/judoka/@pays]"/>

		<tr>
			<td>
				<xsl:choose>
					<xsl:when test="@classementFinal != 0 and @classementFinal &lt; 9">
						<xsl:value-of select="@classementFinal"/>
					</xsl:when>
					<xsl:otherwise>
						<!-- Ajout d'un span pour forcer la couleur neutre du NC -->
						<span class="tas-classement-nc">NC</span>
					</xsl:otherwise>
				</xsl:choose>
			</td>
			<td class="athlete-name">
				<xsl:value-of select="$j1/@nom"/>
				<xsl:if test="$typeCompetition != '1'">
					&nbsp;<xsl:value-of select="$j1/@prenom"/>
				</xsl:if>
			</td>
			<td class="text-muted">
				<xsl:value-of select="$club/nomCourt"/>
			</td>

			<xsl:if test="$niveauCompetition = '3' or $niveauCompetition = '4'">
				<td class="text-muted">
					<xsl:value-of select="$comite/@ID"/>
				</td>
			</xsl:if>
			<xsl:if test="$niveauCompetition = '4'">
				<td class="text-muted">
					<xsl:value-of select="$ligue/nomCourt"/>
				</td>
			</xsl:if>
			<xsl:if test="$niveauCompetition = '5' or $niveauCompetition = '6'">
				<td class="text-muted">
					<xsl:value-of select="$pays/@nom"/>
				</td>
			</xsl:if>
		</tr>
	</xsl:template>
</xsl:stylesheet>