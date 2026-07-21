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
				<meta name="viewport" content="width=device-width,initial-scale=1"/>
				<meta http-equiv="Cache-Control" content="no-cache, no-store, must-revalidate"/>
				<meta http-equiv="Pragma" content="no-cache"/>
				<meta http-equiv="Expires" content="0"/>

				<link type="text/css" rel="stylesheet" href="{concat($cssPath, 'w3.css')}"/>
				<link type="text/css" rel="stylesheet" href="{concat($cssPath, 'style-common.css')}"/>

				<script src="{concat($jsPath, 'site-display.js')}"/>

				<script type="text/javascript">
					<xsl:value-of select="$js"/>
					gUseAutoReload = false;
				</script>

				<title>Suivi Compétition - Prochains Combats</title>
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
					<xsl:with-param name="selectedItem" select="'prochains_combats'"/>
					<xsl:with-param name="pathToImg" select="$imgPath"/>
					<xsl:with-param name="pathToCommon" select="$commonPath"/>
				</xsl:call-template>

				<!-- Div vide pour aligner le contenu avec le bandeau de titre de taille fixe -->
				<div class="w3-container tas-filler-div">&nbsp;</div>

				<!-- CONTENU : Zone "Empty State" modernisée -->
				<xsl:if test="count(competitions/competition)=0 or count(//epreuve)=0">
					<div class="w3-padding">
						<div class="ios-card tas-empty-state">Veuillez patienter, le tirage des épreuves est en cours...</div>
					</div>
				</xsl:if>

				<!-- Boucle globale sur les competitions en cours -->
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
						<script src="{concat($jsPath, 'footer_script.js')}"/>
					</div>
				</xsl:if>

			</body>
		</html>
	</xsl:template>

	<!-- TEMPLATES -->
	<!-- Un bloc -->
	<xsl:template name="competition">
		<xsl:param name="idcompetition"/>
		<xsl:variable name="prefixCompetition" select="concat('ProchainCombatComp',$idcompetition,'ContentPanel')"/>

		<!-- Nom de la competition (Bandeau modernisé) -->
		<div class="tas-competition-bandeau">
			<h4>
				<xsl:value-of select="./titre"/>
			</h4>
		</div>

		<div id="Avancements" class="w3-container pane w3-animate-left">
			<div class="w3-cell-row">
				<!-- Categorie F -->
				<xsl:call-template name="panelEpreuve">
					<xsl:with-param name="sexeCode" select="'F'"/>
					<xsl:with-param name="prefixPanel" select="$prefixCompetition"/>
					<xsl:with-param name="imgPath" select="$imgPath"/>
				</xsl:call-template>
				<!-- Categorie M -->
				<xsl:call-template name="panelEpreuve">
					<xsl:with-param name="sexeCode" select="'M'"/>
					<xsl:with-param name="prefixPanel" select="$prefixCompetition"/>
					<xsl:with-param name="imgPath" select="$imgPath"/>
				</xsl:call-template>
				<!-- Mixte -->
				<xsl:call-template name="panelEpreuve">
					<xsl:with-param name="sexeCode" select="'X'"/>
					<xsl:with-param name="prefixPanel" select="$prefixCompetition"/>
					<xsl:with-param name="imgPath" select="$imgPath"/>
				</xsl:call-template>
			</div>
		</div>

	</xsl:template>

	<!-- Bouton avancement par epreuve -->
	<!-- On ne tient compte que des epreuves pour lesquelles les phases sont créées et sans classement validé -->
	<xsl:template name="prochains_combats_epreuve" match="epreuve">
		<xsl:if test="count(./phases/phase[number(@typePhase) = 1 and number(@etat) > 0 and number(@etat) != 5]) > 0">
			<!-- Bouton style carte iOS (Remplace w3-pale-yellow) -->
			<a class="w3-button w3-block ios-card w3-margin-bottom" href="{concat($competitionPath, @directory, 'feuille_combats.html')}">
				<xsl:value-of select="./@libelle"/>
				<xsl:text>&#32;</xsl:text>
				<xsl:value-of select="./@nom"/>
				<xsl:text>&#32;Poules</xsl:text>
			</a>
		</xsl:if>
		<xsl:if test="count(./phases/phase[number(@typePhase) = 2 and number(@etat) > 0 and number(@etat) != 5]) > 0">
			<!-- Bouton style carte iOS (Remplace w3-pale-yellow) -->
			<a class="w3-button w3-block ios-card w3-margin-bottom" href="{concat($competitionPath, @directory, 'feuille_combats.html')}">
				<xsl:value-of select="./@libelle"/>
				<xsl:text>&#32;</xsl:text>
				<xsl:value-of select="./@nom"/>
				<xsl:text>&#32;Tableau</xsl:text>
			</a>
		</xsl:if>
	</xsl:template>
</xsl:stylesheet>