<?xml version="1.0"?>
<!DOCTYPE xsl:stylesheet [
	<!ENTITY nbsp "&#160;">
	<!ENTITY times "&#215;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:import href="FranceJudo.Metier/Resources/Site/xslt/entete.xslt"/>
	<xsl:import href="FranceJudo.Metier/Resources/Site/xslt/panel_epreuve.xslt"/>

	<xsl:output method="html" indent="yes"/>
	<xsl:param name="style"/>
	<xsl:param name="js"/>
	<xsl:param name="imgPath"/>
	<xsl:param name="jsPath"/>
	<xsl:param name="cssPath"/>
	<xsl:param name="commonPath"/>
	<xsl:param name="competitionPath"/>

	<xsl:key name="combats" match="combat" use="@niveau"/>

	<xsl:variable select="/docroot/SiteConfiguration/@PublierProchainsCombats = 'true'" name="affProchainCombats"/>
	<xsl:variable select="/docroot/SiteConfiguration/@PublierAffectationTapis = 'true'" name="affAffectationTapis"/>
	<xsl:variable select="/docroot/SiteConfiguration/@PublierEngagements = 'true'" name="affEngagements"/>
	<xsl:variable select="/docroot/SiteConfiguration/@PublierStatistiques = 'true'" name="affStatistiques"/>
	<xsl:variable select="/docroot/SiteConfiguration/@Logo" name="logo"/>
	<xsl:variable select="/docroot/SiteConfiguration/@LogoDark" name="logoDark"/>

	<xsl:template match="docroot">
		<xsl:text disable-output-escaping="yes">&lt;!DOCTYPE html&gt;</xsl:text>
		<html>
			<head>
				<META http-equiv="Content-Type" content="text/html; charset=utf-8"/>
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

				<script>
					<xsl:attribute name="src">
						<xsl:value-of select="concat($jsPath, 'site-display.js')"/>
					</xsl:attribute>
				</script>

				<script type="text/javascript">
					<xsl:value-of select="$js"/>
					gUseAutoReload = false;
				</script>
				<title>Suivi Compétition - Avancement</title>
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
					<xsl:with-param name="affActualiser" select="false()"/>
					<xsl:with-param name="selectedItem" select="'avancement'"/>
					<xsl:with-param name="pathToImg" select="$imgPath"/>
					<xsl:with-param name="pathToCommon" select="$commonPath"/>
				</xsl:call-template>

				<!-- CONTENU -->
				<xsl:if test="count(competitions/competition)=0 or count(//epreuve)=0">
					<div class="w3-padding">
						<div class="ios-card tas-empty-state">Veuillez patienter, le tirage des épreuves est en cours...</div>
					</div>
				</xsl:if>

				<!-- Boucle global sur les competitions en cours -->
				<xsl:for-each select="competitions/competition">
					<xsl:if test="count(./epreuve) > 0">
						<xsl:variable name="compet" select="@ID"/>
						<xsl:call-template name="competition">
							<xsl:with-param name="idcompetition" select="$compet"/>
						</xsl:call-template>
					</xsl:if>
				</xsl:for-each>

				<xsl:if test="count(competitions/competition)>0">
					<div class="w3-container w3-center w3-tiny text-muted tas-footnote">
						<script>
							<xsl:attribute name="src">
								<xsl:value-of select="concat($jsPath, 'footer_script.js')"/>
							</xsl:attribute>
						</script>
					</div>
				</xsl:if>
			</body>
		</html>
	</xsl:template>

	<!-- Un bloc -->
	<xsl:template name="competition">
		<xsl:param name="idcompetition"/>

		<xsl:variable name="prefixCompetition">
			<xsl:value-of select="concat('AvancementComp',$idcompetition,'ContentPanel')"/>
		</xsl:variable>

		<div class="tas-competition-bandeau">
			<h4>
				<xsl:value-of select="./titre"/>
			</h4>
		</div>

		<div id="Avancements" class="w3-container pane w3-animate-left tas-competition-panels">
			<div class="w3-row-padding">
				<xsl:call-template name="panelEpreuve">
					<xsl:with-param name="sexeCode" select="'F'"/>
					<xsl:with-param name="prefixPanel" select="$prefixCompetition"/>
					<xsl:with-param name="imgPath" select="$imgPath"/>
				</xsl:call-template>
				<xsl:call-template name="panelEpreuve">
					<xsl:with-param name="sexeCode" select="'M'"/>
					<xsl:with-param name="prefixPanel" select="$prefixCompetition"/>
					<xsl:with-param name="imgPath" select="$imgPath"/>
				</xsl:call-template>
				<xsl:call-template name="panelEpreuve">
					<xsl:with-param name="sexeCode" select="'X'"/>
					<xsl:with-param name="prefixPanel" select="$prefixCompetition"/>
					<xsl:with-param name="imgPath" select="$imgPath"/>
				</xsl:call-template>
			</div>
		</div>
	</xsl:template>

	<!-- Bouton avancement par epreuve -->
	<xsl:template name="avancement_epreuve" match="epreuve">
		<xsl:variable name="nomEpreuve" select="./@nom"/>
		<xsl:variable name="dirEpreuve" select="./@directory" />
		<xsl:variable name="nbPhases" select="count(./phases/phase)" />

		<xsl:choose>
			<!-- CAS 1 : L'ÉPREUVE N'A QU'UNE SEULE PHASE -->
			<xsl:when test="$nbPhases = 1">
				<xsl:for-each select="./phases/phase">
					<xsl:variable name="etat" select="number(@etat)" />
					<xsl:variable name="typePhase" select="number(@typePhase)" />
					<xsl:variable name="libellePhase">
						<xsl:choose>
							<xsl:when test="$typePhase = 1">Poules</xsl:when>
							<xsl:when test="$typePhase = 2">Tableau</xsl:when>
							<xsl:otherwise>Phase</xsl:otherwise>
						</xsl:choose>
					</xsl:variable>
					<xsl:variable name="page">
						<xsl:choose>
							<xsl:when test="$typePhase = 1">poules_resultats.html</xsl:when>
							<xsl:otherwise>tableau_competition.html</xsl:otherwise>
						</xsl:choose>
					</xsl:variable>

					<xsl:choose>
						<xsl:when test="$etat &gt;= 2">
							<a class="ios-list-item" href="{concat($competitionPath, $dirEpreuve, $page)}">
								<xsl:value-of select="$nomEpreuve"/>
								<xsl:text> - </xsl:text>
								<xsl:value-of select="$libellePhase"/>
							</a>
						</xsl:when>
						<xsl:otherwise>
							<div class="ios-list-item ios-list-item-disabled">
								<div>
									<xsl:value-of select="$nomEpreuve"/>
									<xsl:text> - </xsl:text>
									<xsl:value-of select="$libellePhase"/>
								</div>
								<div class="w3-tiny">
									<i>(Tirage en attente)</i>
								</div>
							</div>
						</xsl:otherwise>
					</xsl:choose>
				</xsl:for-each>
			</xsl:when>

			<!-- CAS 2 : L'ÉPREUVE A PLUSIEURS PHASES -->
			<xsl:when test="$nbPhases &gt; 1">
				<div class="ios-multiphase-card">
					<div class="ios-multiphase-header">
						<xsl:value-of select="$nomEpreuve"/>
					</div>
					<div class="ios-multiphase-body">
						<xsl:for-each select="./phases/phase">
							<xsl:sort select="@ordre" data-type="number" order="ascending"/>
							<xsl:variable name="etat" select="number(@etat)" />
							<xsl:variable name="typePhase" select="number(@typePhase)" />
							<xsl:variable name="libellePhase">
								<xsl:choose>
									<xsl:when test="$typePhase = 1">Phase de Poules</xsl:when>
									<xsl:when test="$typePhase = 2">Phase de Tableau</xsl:when>
									<xsl:otherwise>Phase</xsl:otherwise>
								</xsl:choose>
							</xsl:variable>
							<xsl:variable name="page">
								<xsl:choose>
									<xsl:when test="$typePhase = 1">poules_resultats.html</xsl:when>
									<xsl:otherwise>tableau_competition.html</xsl:otherwise>
								</xsl:choose>
							</xsl:variable>

							<xsl:choose>
								<xsl:when test="$etat &gt;= 2">
									<a class="ios-phase-btn" href="{concat($competitionPath, $dirEpreuve, $page)}">
										<xsl:value-of select="$libellePhase"/>
									</a>
								</xsl:when>
								<xsl:otherwise>
									<div class="ios-phase-btn ios-phase-btn-disabled">
										<xsl:value-of select="$libellePhase"/>
										<div class="w3-tiny">
											<i>(Tirage en attente)</i>
										</div>
									</div>
								</xsl:otherwise>
							</xsl:choose>
						</xsl:for-each>
					</div>
				</div>
			</xsl:when>
		</xsl:choose>
	</xsl:template>
</xsl:stylesheet>